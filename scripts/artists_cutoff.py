import xml.etree.ElementTree as ET
from rich.console import Console
from rich.progress import (
    Progress,
    BarColumn,
    TimeRemainingColumn,
    TextColumn,
    ProgressColumn,
)

progress = Progress(
    TextColumn("[bold blue]Parsing XML...", justify="right"),
    BarColumn(bar_width=None),
    "[progress.percentage]{task.percentage:>3.1f}%",
    TimeRemainingColumn(),
)

console = Console()

with console.status("[bold green]Processing XML file...") as status:
    context = ET.iterparse(
        r'D:\\DBMusicDataset\\discogs_20240301_artists.xml', events=("start", "end"))
    context = iter(context)

with console.status("[bold green]Creating list of artists...") as status:
    event, root = next(context)

progress.console.log("[bold green]Processing artists...")
count = 0
maxCount = 5000
task = progress.add_task("[red]Processing...", total=maxCount)

output_file = r'D:\\DBMusicDataset\\artists_correct.xml'
with open(output_file, "w", encoding="utf-8") as output:
    output.write('<?xml version="1.0" encoding="UTF-8"?>\n<artists>\n')

with progress:
    for event, elem in progress.track(context, description="Processing artists..."):
        if count >= maxCount:
            break

        if event == "end" and elem.tag == "artist":
            data_quality = elem.find('data_quality')
            if data_quality is not None and data_quality.text == 'Correct':
                with open(output_file, "a", encoding="utf-8") as output:
                    output.write(ET.tostring(elem, encoding="unicode") + "\n")
                count += 1
                progress.advance(task)
            root.clear()

progress.update(task, completed=count)

with open(output_file, "a", encoding="utf-8") as output:
    output.write('</artists>')
