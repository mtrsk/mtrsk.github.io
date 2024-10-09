import fileinput
import json
import pathlib
import re
import sys

NOTES_DIRECTORY = pathlib.Path("./content/notes")
MARKDOWN_NOTES = NOTES_DIRECTORY.glob("**/*")
STATIC_PATH = pathlib.Path("./static").resolve()
JSON_PATH = pathlib.Path(STATIC_PATH / "graph.json")
PATTERN = re.compile(r"\[BROKEN LINK: (.+)\]")

with open(JSON_PATH.resolve()) as f:
    graph_list = json.loads(f.read())
graph = {k["id"].replace("\"", ""):k["lnk"] for k in graph_list["nodes"]}

def rewrite(lnk: str) -> str:
    year = lnk[:4]
    filename = lnk[15:]
    title = filename.replace("_", " ").title()
    #return f"[{title}]({{< relref \"../notes/{filename}.md\" >}})"
    #return f"[{title}]" + "({{< relref " + f"../notes/{filename}.md" + " >}})"
    #return f"[{title}](../notes/{filename}.md)"
    return f"[{title}](/notes/{year}/{filename}/)"

def fix_links(path: pathlib.PosixPath) -> None:
    for line in fileinput.input([path], inplace=True):
        matches = re.findall(PATTERN, line)
        if len(matches) > 0:
            for item in matches:
                if item in graph:
                    #print(rewrite(graph[item]))
                    line = re.sub(PATTERN, rewrite(graph[item]), line, count=1)
        sys.stdout.write(line)

if __name__ == "__main__":
    list(map(fix_links, [n.resolve() for n in MARKDOWN_NOTES if n.is_file()]))
