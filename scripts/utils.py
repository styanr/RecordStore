import re
import random

random.seed(42)

def remove_brackets(text):
    pattern = r'\[\w=(.+?)\]'

    # remove brackets, e.g. [w=100] -> 100
    return re.sub(pattern, r'\1', text)


def fix_date(date):
    if not date or date == '0':
        return None
    date_parts = date.split('-')
    if len(date_parts) == 1:
        return f"{date_parts[0]}-01-01"
    if date_parts[1] == '00':
        return f"{date_parts[0]}-01-01"
    if date_parts[2] == '00':
        return f"{date_parts[0]}-{date_parts[1]}-01"
    return '-'.join(date_parts)


def random_price():
    return round(random.uniform(5, 50), 2)


def random_quantity():
    return random.randint(1, 200)
