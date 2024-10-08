import fileinput
import pathlib
import re
import sys

NOTES_DIRECTORY = pathlib.Path("./content/notes")
MARKDOWN_NOTES = NOTES_DIRECTORY.glob("**/*")
PATTERN = re.compile(r"[0-9]{14}\-")

def fix_links(path: pathlib.PosixPath) -> None:
    for line in fileinput.input([path], inplace=True):
        if "relref \"../notes/" in line.strip():
            line = line.replace("relref \"../notes/", f"relref \"../content/notes/")
            line = re.sub(PATTERN, "", line)
        sys.stdout.write(line)

if __name__ == "__main__":
    list(map(fix_links, [n.resolve() for n in MARKDOWN_NOTES if n.is_file()]))
