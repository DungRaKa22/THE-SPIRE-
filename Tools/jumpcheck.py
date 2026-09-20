#!/usr/bin/env python3
"""Deterministic jump-budget gate for THE SPIRE level documents.

Reads the platform tables in Docs/TheSpire-Level-Sector*.md and re-derives every
documented link with the constants locked in Docs/memory/01-Architecture.md:

    g_eff    = 9.81 * gravityScale(3.0)            = 29.43 m/s^2
    v(p)     = lerp(4.0, 13.0, p)
    horiz(p) = 6.0 * lerp(0.4, 1.0, p)
    H(p)     = v(p)^2 / (2 g)
    t2       = (v + sqrt(v^2 - 2 g dy)) / g        falling branch (Jump King style)
    D(dy, p) = horiz(p) * t2
    legal    : dy <= 0.92 H(p)  AND  dx <= 0.85 D(dy, p)

Checks performed
    HARD   - no charge level makes the link reachable at all, or p_max < p_min
    WINDOW - convention v2: the legal charge range [p_min, p_max] is narrower than 0.10
    HARD - no charge level makes the link reachable at all
    CONV - documented p equals the HEIGHT-ONLY minimum instead of the real one.
           Sector 1 and 2 tables are written that way today, so this is reported as a
           column-convention finding, not a design error.  p_full is the real minimum.
    WEAK - documented p matches neither the full nor the height-only minimum
    GEOM - documented dy/dx must equal the values recomputed from x/y/width of
           consecutive STATIC platforms.  Understated dx is the dangerous direction
           (the link looks easier than it is) and is labelled RISKY.
    NUM  - platform numbers strictly increase, no duplicates
    COIN - groups of five (PlatformCoins rule) per sector
    TIGHT- the hardest links per sector by p_full

Exit code 0 = no HARD and no GEOM failure.  --strict also fails on CONV/WEAK.

Usage:  python3 Tools/jumpcheck.py [--strict] [--report Docs/TheSpireJumpCheck.txt]
"""

from __future__ import annotations

import argparse
import math
import re
import sys
from pathlib import Path

GRAVITY = 9.81 * 3.0
V_MIN, V_MAX = 4.0, 13.0
HORIZ_LO, HORIZ_HI = 0.4, 1.0
HORIZ_SPEED = 6.0
DY_MARGIN, DX_MARGIN = 0.92, 0.85
TOL_P = 0.05             # validator criterion of Sector 1: |p_doc - p_found| <= 0.05
TOL_HEIGHT_ONLY = 0.011  # rounding slack when p documents the height-only minimum
TOL_GEOM_DY = 0.02
TOL_GEOM_DX = 0.05
TIGHT = 0.75             # p_full at or above this counts as a hard link
WINDOW_MIN = 0.10        # convention v2: p_max - p_min narrower than this is a tight link

SECTOR_FILES = [
    "Docs/TheSpire-Level-Sector1.md",
    "Docs/TheSpire-Level-Sector2.md",
    "Docs/TheSpire-Level-Sector3.md",
]

PLACEHOLDERS = {"", "-", "--", "\u2014", "\u2013", "n/a", "na", "?"}


def speed(p: float) -> float:
    return V_MIN + (V_MAX - V_MIN) * p


def horiz(p: float) -> float:
    return HORIZ_SPEED * (HORIZ_LO + (HORIZ_HI - HORIZ_LO) * p)


def peak(p: float) -> float:
    return speed(p) ** 2 / (2 * GRAVITY)


def reach(dy: float, p: float) -> float:
    """Horizontal reach when landing dy above the takeoff surface."""
    v = speed(p)
    disc = v * v - 2 * GRAVITY * dy
    if disc < 0:
        return 0.0
    return horiz(p) * (v + math.sqrt(disc)) / GRAVITY


def min_charge(dy: float, dx: float) -> float:
    """Smallest p in [0,1] satisfying both budget constraints."""
    for step in range(0, 1001):
        p = step / 1000.0
        if dy <= DY_MARGIN * peak(p) and dx <= DX_MARGIN * reach(dy, p):
            return p
    return float("inf")


def min_charge_height(dy: float) -> float:
    """Smallest p in [0,1] satisfying the vertical rule alone."""
    for step in range(0, 1001):
        p = step / 1000.0
        if dy <= DY_MARGIN * peak(p):
            return p
    return float("inf")


def number(text: str) -> float | None:
    text = text.strip().replace("\u2212", "-").replace(",", ".")
    if text.lower() in PLACEHOLDERS:
        return None
    m = re.match(r"^-?\d+(\.\d+)?$", text)
    return float(m.group(0)) if m else None


class Row:
    def __init__(self, sector: str, line_no: int, cells: list[str], cols: dict[str, int]):
        self.sector = sector
        self.line_no = line_no

        def cell(name: str) -> str:
            idx = cols.get(name)
            return cells[idx] if idx is not None and idx < len(cells) else ""

        self.id = number(cell("#"))
        self.name = (cell("Tên") or cell("Ten")).replace("**", "")
        self.kind = cell("Loại") or cell("Loai")
        self.x = number(cell("x"))
        self.width = number(cell("rộng")) or number(cell("rong"))
        self.y = number(cell("y đỉnh")) or number(cell("y"))
        self.y_hi = number(cell("y_hi"))
        self.dy = number(cell("Δy"))
        self.dx = number(cell("Δx"))
        # Convention v2 renames `p` to `p_min` and adds the optional ceiling `p_max`.
        self.p = number(cell("p_min")) if number(cell("p_min")) is not None else number(cell("p"))
        self.p_max = number(cell("p_max"))

    @property
    def standing_y(self) -> float | None:
        return self.y if self.y is not None else self.y_hi

    @property
    def is_static(self) -> bool:
        return self.kind.strip().lower() in ("tĩnh", "tinh", "static", "")

    @property
    def is_shortcut(self) -> bool:
        low = (self.kind + " " + self.name).lower()
        return "updraft" in low or "đường tắt" in low or "duong tat" in low

    @property
    def label(self) -> str:
        return f"{int(self.id):03d} {self.name}".strip()

    def where(self) -> str:
        return f"line {self.line_no}"


def parse_sector(path: Path) -> list[Row]:
    rows: list[Row] = []
    cols: dict[str, int] = {}
    for i, line in enumerate(path.read_text(encoding="utf-8").splitlines(), start=1):
        if not line.lstrip().startswith("|"):
            cols = {}
            continue
        cells = [c.strip() for c in line.strip().strip("|").split("|")]
        if set("".join(cells)) <= set("-: "):
            continue
        if "#" in cells:
            cols = {name: idx for idx, name in enumerate(cells)}
            continue
        if not cols:
            continue
        row = Row(path.name, i, cells, cols)
        if row.id is not None:
            rows.append(row)
    return rows


def check_sector(path: Path) -> dict[str, list[str]]:
    rows = parse_sector(path)
    found: dict[str, list[str]] = {k: [] for k in ("HARD", "WINDOW", "CONV", "WEAK", "GEOM", "NUM", "COIN", "TIGHT")}
    if not rows:
        found["HARD"].append("no platform table found")
        return found

    seen: dict[int, str] = {}
    previous_id: int | None = None
    tight: list[tuple[float, str]] = []

    for row in rows:
        rid = int(row.id)
        if rid in seen:
            found["NUM"].append(f"{row.where()} platform {rid:03d} duplicated (also {seen[rid]})")
        else:
            seen[rid] = row.where()
        if previous_id is not None and rid <= previous_id:
            found["NUM"].append(f"{row.where()} platform {rid:03d} does not increase after {previous_id:03d}")
        previous_id = rid

        if row.dy is None or row.dx is None or row.p is None:
            continue
        p_full = min_charge(row.dy, row.dx)
        if p_full == float("inf"):
            found["HARD"].append(
                f"{row.where()} {row.label}: unreachable at ANY charge (dy={row.dy:.2f}, dx={row.dx:.2f})"
            )
            continue
        if row.dy > DY_MARGIN * peak(1.0) + 1e-9:
            found["HARD"].append(
                f"{row.where()} {row.label}: dy={row.dy:.2f} above the 0.92H ceiling "
                f"({DY_MARGIN * peak(1.0):.2f} m)"
            )
        if p_full >= TIGHT:
            tight.append((p_full, row.label))
        if abs(row.p - p_full) <= TOL_P:
            continue
        p_height = min_charge_height(row.dy)
        if abs(row.p - p_height) <= TOL_HEIGHT_ONLY:
            found["CONV"].append(
                f"{row.where()} {row.label}: p={row.p:.2f} is the HEIGHT-only minimum; "
                f"the real minimum for dy={row.dy:.2f} dx={row.dx:.2f} is p={p_full:.2f}"
            )
        else:
            found["WEAK"].append(
                f"{row.where()} {row.label}: p={row.p:.2f} matches neither minimum "
                f"(full {p_full:.2f}, height-only {p_height:.2f})"
            )

    static = [r for r in rows if r.is_static and r.standing_y is not None]
    for prev, row in zip(static, static[1:]):
        if int(row.id) != int(prev.id) + 1 or row.dy is None or row.dx is None:
            continue
        if prev.width is None or prev.x is None or row.x is None:
            continue
        dy = row.standing_y - prev.standing_y
        dx = max(0.0, abs(row.x - prev.x) - prev.width / 2)
        if abs(dy - row.dy) > TOL_GEOM_DY:
            found["GEOM"].append(
                f"{row.where()} {row.label}: dy documented {row.dy:.2f}, geometry says {dy:.2f} "
                f"(from {int(prev.id):03d} {prev.name})"
            )
        if abs(dx - row.dx) > TOL_GEOM_DX:
            direction = "RISKY - link is harder than documented" if dx > row.dx else "conservative"
            found["GEOM"].append(
                f"{row.where()} {row.label}: dx documented {row.dx:.2f}, geometry says {dx:.2f} "
                f"({direction}, from {int(prev.id):03d} {prev.name})"
            )

    groups = len(rows) // 5
    found["COIN"].append(
        f"{len(rows)} numbered platforms -> {groups} coin groups, {len(rows) % 5} platform(s) outside a group"
    )
    for p_full, label in sorted(tight, reverse=True)[:5]:
        found["TIGHT"].append(f"p_full={p_full:.2f}  {label}")
    return found


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--report", default="Docs/TheSpireJumpCheck.txt")
    parser.add_argument("--strict", action="store_true", help="also fail on CONV/WEAK findings")
    args = parser.parse_args()

    root = Path(__file__).resolve().parent.parent
    order = ("HARD", "WINDOW", "GEOM", "WEAK", "CONV", "NUM", "TIGHT", "COIN")
    titles = {
        "HARD": "unreachable links",
        "WINDOW": "charge windows narrower than convention v2 allows",
        "GEOM": "table geometry mismatches",
        "WEAK": "p column matches no minimum",
        "CONV": "p column documents the height-only minimum",
        "NUM": "numbering",
        "TIGHT": "hardest links",
        "COIN": "coin groups",
    }
    out: list[str] = []
    totals = {k: 0 for k in order}

    for rel in SECTOR_FILES:
        path = root / rel
        out.append(f"=== {path.name} ===")
        if not path.exists():
            out.append("MISSING FILE")
            continue
        found = check_sector(path)
        for key in order:
            items = found[key]
            totals[key] += len(items) if key not in ("TIGHT", "COIN") else 0
            if not items:
                continue
            out.append(f"-- {key}: {titles[key]} ({len(items)})")
            out += [f"   {item}" for item in items]
        out.append("")

    hard, geom = totals["HARD"], totals["GEOM"]
    verdict = (
        f"FAIL: {hard} unreachable link(s), {geom} geometry mismatch(es)."
        if hard or geom
        else "PASS: every documented link is reachable and the tables are self-consistent."
    )
    header = [
        "THE SPIRE - deterministic jump-budget gate (Tools/jumpcheck.py)",
        f"constants: g_eff={GRAVITY:.2f}, v=lerp({V_MIN:g},{V_MAX:g},p), horiz={HORIZ_SPEED:g}*lerp(0.4,1,p), "
        f"margins dy<={DY_MARGIN}H, dx<={DX_MARGIN}D",
        f"findings: HARD={totals['HARD']} WINDOW={totals['WINDOW']} GEOM={totals['GEOM']} "
        f"WEAK={totals['WEAK']} CONV={totals['CONV']} NUM={totals['NUM']}",
        verdict,
        "",
    ]
    text = "\n".join(header + out + [verdict])
    print(text)
    if args.report:
        target = root / args.report
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_text(text + "\n", encoding="utf-8")
    if hard or geom:
        return 1
    return 1 if args.strict and (totals["WEAK"] or totals["CONV"]) else 0


if __name__ == "__main__":
    sys.exit(main())
