import xml.etree.ElementTree as ET
from time import sleep
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
    tree = ET.parse(r'D:\DBMusicDataset\discogs_20240301_artists.xml')


root = tree.getroot()
with console.status("[bold green]Creating parent map...") as status:
    parent_map = dict((c, p) for p in tree.iter() for c in p)

with console.status("[bold green]Creating list of artists...") as status:
    iterator = list(root.iter('artist'))

progress.console.log("[bold green]Processing artists...")


count = 0
maxCount = 5000

task = progress.add_task("[red]Processing...", total=maxCount)

new_root = ET.Element("artists")

with progress:
    for artist in progress.track(iterator, description="Processing artists..."):
        if count >= maxCount:
            break

        progress.console.log(f"Processing {artist.find('name').text}")
        data_quality = artist.find('data_quality')

        if data_quality.text == 'Correct':
            progress.console.log(
                f"Adding {artist.find('name').text} to new root")
            new_root.append(artist)

            count += 1
            progress.advance(task)


progress.update(task, completed=count)


tree = ET.ElementTree(new_root)

tree.write(r'D:\DBMusicDataset\artists_correct.xml')
