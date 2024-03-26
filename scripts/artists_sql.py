import re
import xml.etree.ElementTree as ET
from sqlobject.converters import sqlrepr
from rich.progress import Progress, BarColumn, TimeRemainingColumn, TextColumn

# Initialize Rich progress bar
progress = Progress(
    TextColumn("[bold blue]Parsing XML...", justify="right"),
    BarColumn(bar_width=None),
    "[progress.percentage]{task.percentage:>3.1f}%",
    TimeRemainingColumn(),
)


class Artist:
    def __init__(self, name, profile):
        self.name = name
        self.description = profile

    def __str__(self):
        return f"Name: {self.name}\nProfile: {self.description}\n"

    def __repr__(self):
        return f"Name: {self.name}\nProfile: {self.description}\n"

# Define function to remove brackets from text


def remove_brackets(text):
    pattern = r'\\?\[(l=|a=)(.+?)\\]'
    return re.sub(pattern, r'\2', text)

# Parse individual artist from XML


def parse_artist(artist):
    name = sqlrepr(artist.find('name').text, 'postgres')
    profile = artist.find('profile').text
    profile = sqlrepr(remove_brackets(profile),
                      'postgres') if profile else None
    return Artist(name, profile)

# Parse XML file


def parse_xml(file_path):
    tree = ET.parse(file_path)
    root = tree.getroot()
    artists = [parse_artist(artist) for artist in root.iter('artist')]
    return artists

# Main function to execute parsing and generate SQL file


def main():
    with progress.console.status("[bold green]Processing XML file..."):
        artists = parse_xml(r'D:\DBMusicDataset\artists_correct.xml')

    task = progress.add_task("[red]Processing...", total=len(artists))
    with progress:
        with open('artists.sql', 'w', encoding='utf-8') as f:
            for artist in artists:
                if artist.description:
                    f.write(
                        f"INSERT INTO artists (name, description) VALUES ({artist.name}, {artist.description});\n")
                else:
                    f.write(
                        f"INSERT INTO artists (name) VALUES ({artist.name});\n")
                progress.advance(task)


if __name__ == '__main__':
    main()
