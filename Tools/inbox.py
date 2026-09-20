#!/usr/bin/env python3
"""Merge one inbox ticket (agent delivery) into the repository, safely.

Agents cannot push (see Docs/team/01-PIPELINE.md). They hand over a folder:

    T-0xx/
    ├── report.md      first line should be  baseline: <7-char sha>
    └── files/         complete files, keeping their path inside the repo

This tool takes that folder, shows what would change, refuses anything outside
the allowed scope, and (only with --write) copies the files into the repo.

Usage
    python3 Tools/inbox.py T-009                 # dry run: diff + gates, no change
    python3 Tools/inbox.py T-009 --write         # apply into the repo
    python3 Tools/inbox.py T-009 --inbox D:\\git\\inbox
"""

from __future__ import annotations

import argparse
import difflib
import re
import subprocess
import sys
import tempfile
from pathlib import Path

ALLOWED_PREFIXES = ("Docs/", "Tools/", "Assets/Scripts/", "Assets/Editor/", ".github/")
ALLOWED_FILES = ("README.md", ".gitignore")
# Never accepted from an inbox ticket: scenes and project settings are owned by
# the builder ticket / the project owner, and binary blobs are reviewed by hand.
FORBIDDEN_PREFIXES = ("Assets/Scenes/", "ProjectSettings/", "Packages/", "Assets/Resources/")
MAX_BYTES = 1_000_000
CS_META = "fileFormatVersion: 2\nguid: {guid}\n"


def find_ticket(args) -> Path:
    candidates = []
    if args.inbox:
        candidates.append(Path(args.inbox) / args.ticket)
    candidates += [
        Path(args.inbox or "../inbox") / args.ticket,
        Path("../inbox") / args.ticket,
        Path("..") / args.ticket,
        Path(args.ticket),
    ]
    for candidate in candidates:
        if (candidate / "files").is_dir():
            return candidate.resolve()
    raise SystemExit(
        "inbox: không tìm thấy thư mục ticket.\n"
        "Đã thử:\n  " + "\n  ".join(str(c) for c in candidates) + "\n"
        "Cần có <ticket>/files/ và <ticket>/report.md. Dùng --inbox <đường dẫn> nếu để chỗ khác."
    )


def classify(rel: str, repo: Path) -> tuple[str, str]:
    """Return (verdict, reason). verdict in ok|forbidden|toolarge|nomedia."""
    if any(rel.startswith(p) for p in FORBIDDEN_PREFIXES):
        return "forbidden", "ngoài phạm vi ticket (scene/cấu hình project do builder hoặc chủ dự án sở hữu)"
    if not (rel.startswith(ALLOWED_PREFIXES) or rel in ALLOWED_FILES):
        return "forbidden", "đường dẫn không nằm trong phạm vi cho phép"
    if (repo / rel).is_file() and (repo / rel).stat().st_size > MAX_BYTES:
        return "toolarge", f"file hiện tại lớn hơn {MAX_BYTES // 1000} KB"
    return "ok", ""


def new_meta_for(path: Path) -> str | None:
    """Unity meta for a brand new file. Scripts are safe to write; textures need
    the project's own importer settings, so those are left to Unity."""
    if path.suffix == ".cs":
        import uuid
        return CS_META.format(guid=uuid.uuid4().hex)
    if path.suffix == ".md":
        return None  # Unity ignores markdown, no meta is created
    return None


def gate(repo: Path, write: bool) -> str:
    report = None if write else Path(tempfile.gettempdir()) / "inbox-gate.txt"
    cmd = [sys.executable, "Tools/jumpcheck.py"]
    if report:
        cmd += ["--report", str(report)]
    done = subprocess.run(cmd, cwd=repo, capture_output=True, text=True)
    lines = [l for l in done.stdout.splitlines() if l.startswith(("findings:", "FAIL:", "PASS:"))]
    seen, unique = set(), []
    for line in lines:
        if line not in seen:
            seen.add(line); unique.append(line)
    return "\n".join(unique) if unique else "(không chạy được: " + done.stderr.strip()[:200] + ")"


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("ticket", help="ví dụ T-009")
    parser.add_argument("--inbox", help="thư mục chứa các ticket (mặc định ../inbox)")
    parser.add_argument("--write", action="store_true", help="ghi thật vào repo")
    args = parser.parse_args()

    repo = Path(__file__).resolve().parent.parent
    ticket = find_ticket(args)
    print(f"ticket : {ticket}")
    print(f"repo   : {repo}")
    print(f"mode   : {'WRITE (có thay đổi)' if args.write else 'DRY RUN (không thay đổi gì)'}")

    report = ticket / "report.md"
    if report.is_file():
        head = report.read_text(encoding="utf-8").splitlines()[:6]
        baseline = next((l for l in head if l.lower().startswith("baseline")), None)
        print("report : " + (baseline or "THIẾU dòng baseline: <sha> (Arena phải kiểm tay)"))
    else:
        print("report : THIẾU report.md")

    added, changed, same, rejected = [], [], [], []
    for src in sorted((ticket / "files").rglob("*")):
        if not src.is_file():
            continue
        rel = str(src.relative_to(ticket / "files")).replace("\\", "/")
        verdict, reason = classify(rel, repo)
        if verdict != "ok":
            rejected.append((rel, reason))
            continue
        dst = repo / rel
        if not dst.exists():
            added.append(rel)
            if args.write:
                dst.parent.mkdir(parents=True, exist_ok=True)
                dst.write_bytes(src.read_bytes())
                meta = new_meta_for(dst)
                if meta:
                    Path(str(dst) + ".meta").write_text(meta, encoding="utf-8")
        else:
            old = dst.read_text(encoding="utf-8", errors="replace").splitlines()
            new = src.read_text(encoding="utf-8", errors="replace").splitlines()
            if old == new:
                same.append(rel)
                continue
            changed.append(rel)
            diff = list(difflib.unified_diff(old, new, f"repo/{rel}", "inbox/" + rel, lineterm="", n=1))
            plus = sum(1 for l in diff if l.startswith("+") and not l.startswith("+++"))
            minus = sum(1 for l in diff if l.startswith("-") and not l.startswith("---"))
            print(f"\n--- {rel}  (+{plus}/-{minus})")
            for line in diff[:14]:
                print("   " + line)
            if len(diff) > 14:
                print(f"   … còn {len(diff) - 14} dòng diff")
            if args.write:
                dst.write_bytes(src.read_bytes())

    print("\nTỔNG KẾT")
    print(f"  nhận mới : {len(added)} {added if added else ''}")
    print(f"  sửa      : {len(changed)} {changed if changed else ''}")
    print(f"  y nguyên  : {len(same)}")
    if rejected:
        print(f"  TỪ CHỐI  : {len(rejected)}")
        for rel, reason in rejected:
            print(f"    - {rel}: {reason}")

    if any(Path(rel).name.startswith("TheSpire-Level-Sector") for rel in added + changed):
        print("\nCỔNG")
        print(gate(repo, args.write))
    else:
        print("\nCỔNG: ticket không đụng bảng bệ, bỏ qua jumpcheck.")

    if args.write:
        print("\nĐã ghi vào repo. Việc tiếp: git add -A && git commit -m \"T-xxx: …\" && git push")
    else:
        print("\nChưa ghi gì. Chạy lại với --write khi diff đúng như mong đợi.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
