import requests
import psycopg2 as pg

DB_NAME = "RecordStoreDB"
DB_USER = "postgres"
DB_PASSWORD = "Jd35B^1-Wa_/}0,"
DB_HOST = "localhost"
DB_PORT = "5432"


def connect():
    return pg.connect(dbname=DB_NAME,
                      user=DB_USER,
                      password=DB_PASSWORD,
                      host=DB_HOST,
                      port=DB_PORT)

url = "https://api.novaposhta.ua/v2.0/json/"
obj = {
    'modelName': 'Address',
    'calledMethod': 'getAreas',
    'methodProperties': {}
}

response = requests.post(url, json=obj)

data = response.json()['data']

regions = []

for region in data[1::]: # skip first element
    regions.append(region['Description'])

print(regions)

def load_regions():
    conn = connect()
    cur = conn.cursor()

    for region in regions:
        cur.execute("INSERT INTO region (region_name) VALUES (%s)", (region,))

    conn.commit()
    cur.close()
    conn.close()

load_regions()