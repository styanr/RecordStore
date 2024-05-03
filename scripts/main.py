from data_sql import run as data_sql
from business_sql import run as business_sql


def main():
    data_sql()
    business_sql()


if __name__ == '__main__':
    main()
