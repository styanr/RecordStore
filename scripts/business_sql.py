import psycopg2 as pg
import bcrypt
from faker import Faker
import random
import datetime
from dateutil.relativedelta import relativedelta
import decimal
import numpy as np

fake = Faker('uk_UA')

DB_NAME = "RecordStoreDB"
DB_USER = "postgres"
DB_PASSWORD = "Jd35B^1-Wa_/}0,"
DB_HOST = "localhost"
DB_PORT = "5432"

NUM_USERS = 2000
BASE_MONTHLY_ORDER_VOLUME = 800


def connect():
    return pg.connect(dbname=DB_NAME,
                      user=DB_USER,
                      password=DB_PASSWORD,
                      host=DB_HOST,
                      port=DB_PORT)


def generate_users():
    users = []
    for _ in range(NUM_USERS):
        users.append((fake.first_name(), fake.last_name(),
                     fake.email(), fake.password()))

        # check for duplicate emails
        while len(users) != len(set([user[2] for user in users])):
            users.pop()
            users.append((fake.first_name(), fake.last_name(),
                          fake.email(), fake.password()))

    return users


def write_users(users):
    # write login data to file

    with open("login_data.txt", "w") as file:
        for user in users:
            file.write(f"{user[2]} {user[3]}\n")


def insert_users(users, cursor):
    cursor.execute("SELECT id FROM role WHERE role_name = 'user'")
    role_id = cursor.fetchone()[0]

    cursor.execute("SELECT id FROM role WHERE role_name = 'admin'")
    admin_role_id = cursor.fetchone()[0]

    admin_password = bcrypt.hashpw("admin".encode(
        'utf-8'), bcrypt.gensalt())  # Hash the password
    admin_password = admin_password.decode('utf-8')
    admin_password = admin_password.replace("$2b$", "$2a$")

    cursor.execute("INSERT INTO app_user (first_name, last_name, email, password, role_id, created_at) VALUES (%s, %s, %s, %s, %s, %s) RETURNING id",
                   ("Admin", "Admin", "admin@admin.com", admin_password, admin_role_id, datetime.date.today() - relativedelta(years=2)))

    # add employee
    cursor.execute("SELECT id FROM role WHERE role_name = 'employee'")
    employee_role_id = cursor.fetchone()[0]

    employee_password = bcrypt.hashpw("employee".encode(
        'utf-8'), bcrypt.gensalt())  # Hash the password
    employee_password = employee_password.decode('utf-8')
    employee_password = employee_password.replace("$2b$", "$2a$")

    cursor.execute("INSERT INTO app_user (first_name, last_name, email, password, role_id, created_at) VALUES (%s, %s, %s, %s, %s, %s) RETURNING id",
                   ("Employee", "Employee", "employee@employee.com", employee_password, employee_role_id, datetime.date.today() - relativedelta(years=2)))

    user_ids = []
    hashed_password = bcrypt.hashpw("password".encode(
        'utf-8'), bcrypt.gensalt())  # Hash the password
    hashed_password = hashed_password.decode('utf-8')
    hashed_password = hashed_password.replace("$2b$", "$2a$")
    for user in users:
        created_at = fake.date_time_between(start_date="-3y", end_date="-2y")

        cursor.execute(
            "INSERT INTO app_user (first_name, last_name, email, password, role_id, created_at) VALUES (%s, %s, %s, %s, %s, %s) RETURNING id",
            (user[0], user[1], user[2], hashed_password, role_id, created_at)
        )

        user_id = cursor.fetchone()[0]
        user_ids.append(user_id)

        print(f"User {user_id} inserted")

    return user_ids


# id, record_id, description, price, created_at, updated_at
def get_products(cursor):
    cursor.execute("select * from product")

    products = cursor.fetchall()

    return products


def generate_orders(user_ids, products):
    orders = []

    seasonal_variability = [0.8, 0.9, 1.0, 1.1,
                            1.2, 1.3, 1.1, 1.0, 1.2, 1.4, 1.6, 1.8]

    population_data = [
        ("Автономна Республіка Крим", 1967.2),
        ("Вінницька область", 1507.738),
        ("Волинська область", 1020.77),
        ("Дніпропетровська область", 3093.151),
        ("Донецька область", 4056.405),
        ("Житомирська область", 1177.564),
        ("Закарпатська область", 1243.721),
        ("Запорізька область", 1636.322),
        ("Івано-Франківська область", 1350.565),
        ("Київська область", 1795.542),
        ("Кіровоградська область", 902.275),
        ("Луганська область", 2101.653),
        ("Львівська область", 2476.113),
        ("Миколаївська область", 1090.492),
        ("Одеська область", 2349.749),
        ("Полтавська область", 1350.564),
        ("Рівненська область", 1140.902),
        ("Сумська область", 1034.364),
        ("Тернопільська область", 1020.953),
        ("Харківська область", 2596.25),
        ("Херсонська область", 1000.37),
        ("Хмельницька область", 1227.474),
        ("Черкаська область", 1159.2),
        ("Чернівецька область", 889.928),
        ("Чернігівська область", 957.665),
        ("Київ", 2950.702),
        ("Севастополь", 385.9),
    ]

    population_sum = sum([population for _, population in population_data])

    regions = [region for region, _ in population_data]
    weights = [population / population_sum for _,
               population in population_data]

    product_popularity = {product[0]: random.gauss(
        1, 0.5) for product in products}

    product_rating_variability = {
        product[0]: random.uniform(0.5, 1.5) for product in products}

    for month in range(1, 13):
        variability = random.uniform(0.8, 1.2)

        upswing = (1 + 0.5 * (month / 12)) * variability * \
            seasonal_variability[month - 1]

        monthly_order_volume = int(BASE_MONTHLY_ORDER_VOLUME * upswing)

        total_orders = 0
        for day in range(1, 29):
            day_variability = random.uniform(0.8, 1.2)
            num_orders = int(monthly_order_volume / 28 * day_variability)
            total_orders += num_orders

            for _ in range(num_orders):
                user_id = random.choice(user_ids)

                order_products_ids = random.sample(
                    products, max(round(random.gauss(1, 1)), 1))
                order_products = []

                reviews = []
                for product in order_products_ids:
                    quantity = max(random.gauss(
                        1, 0.5) * product_popularity[product[0]], 1)
                    price = product[4]
                    order_products.append((product[0], quantity, price))

                    if random.random() > 0.3:
                        rating = round(
                            min(max(random.gauss(3, 1) * product_rating_variability[product[0]], 1), 5))
                        description = fake.sentence()
                        reviews.append(
                            (product[0], user_id, rating, description))

                city = fake.city_name()
                street = fake.street_name()
                building = fake.building_number()
                region = np.random.choice(regions, p=weights)
                apartment = random.randint(
                    1, 100) if random.random() > 0.5 else None

                created_at = datetime.date.today() - relativedelta(years=2) + \
                    relativedelta(months=month, days=day)

                orders.append((user_id, order_products, city,
                              street, building, apartment, region, created_at, reviews))

    return orders


def check_restock(cursor):
    cursor.execute("select * from get_reorder_ids()")
    restock_ids = cursor.fetchall()

    cursor.execute("select * from inventory")

    return restock_ids


def insert_restock(cursor, restock_ids, supplier_ids, current_date):
    if not restock_ids:
        return

    total = 0
    purchase_order_lines = []

    for product_id in [restock_id[0] for restock_id in restock_ids]:
        price = get_product_price(cursor, product_id)
        inventory = get_product_inventory(cursor, product_id)
        qty_to_order = calculate_quantity_to_order(inventory)
        line_total = qty_to_order * (price * decimal.Decimal(0.8))
        total += line_total
        purchase_order_lines.append((product_id, qty_to_order))

    purchase_order_id = insert_purchase_order(
        cursor, current_date, total, supplier_ids)

    for product_id, qty_to_order in purchase_order_lines:
        insert_purchase_order_line(
            cursor, purchase_order_id, product_id, qty_to_order)


def get_product_price(cursor, product_id):
    cursor.execute("SELECT price FROM product WHERE id = %s", (product_id,))
    return cursor.fetchone()[0]


def get_product_inventory(cursor, product_id):
    cursor.execute(
        "SELECT quantity, restock_level FROM inventory WHERE product_id = %s", (product_id,))
    return cursor.fetchone()


def calculate_quantity_to_order(inventory):
    quantity, restock_level = inventory
    return restock_level - quantity + random.randint(10, 100)


def insert_purchase_order(cursor, current_date, total, supplier_ids):
    cursor.execute("INSERT INTO purchase_order (created_at, total, supplier_id) VALUES (%s, %s, %s) RETURNING id",
                   (current_date, total, random.choice(supplier_ids)))
    return cursor.fetchone()[0]


def insert_purchase_order_line(cursor, purchase_order_id, product_id, quantity):
    cursor.execute("INSERT INTO purchase_order_line (purchase_order_id, product_id, quantity) VALUES (%s, %s, %s)",
                   (purchase_order_id, product_id, quantity))


def generate_suppliers():
    suppliers = []
    for _ in range(20):
        suppliers.append((fake.company(), fake.phone_number(), fake.address()))

    return suppliers


def insert_suppliers(suppliers, cursor):
    supplier_ids = []
    for supplier in suppliers:
        cursor.execute("insert into supplier (name, phone, address) values (%s, %s, %s) returning id",
                       (supplier[0], supplier[1], supplier[2]))
        supplier_id = cursor.fetchone()[0]
        supplier_ids.append(supplier_id)

    return supplier_ids


def insert_orders(orders, cursor, supplier_ids):
    length = len(orders)

    i = 0
    for order in orders:
        user_id = order[0]
        order_products = order[1]
        city = order[2]
        street = order[3]
        building = order[4]
        apartment = order[5]
        region = order[6]
        created_at = order[7]

        cursor.execute("insert into shop_order (user_id, city, street, building, apartment, region, created_at) values (%s, %s, %s, %s, %s, %s, %s) returning id",
                       (user_id, city, street, building, apartment, region, created_at))

        order_id = cursor.fetchone()[0]

        current_date = datetime.date.today()
        for product in order_products:
            current_date = created_at

            product_id = product[0]
            quantity = product[1]

            try:
                cursor.execute("insert into order_line (shop_order_id, product_id, quantity, price, created_at) values (%s, %s, %s, %s, %s)",
                               (order_id, product_id, quantity, product[2], created_at))
            except Exception as e:
                if "positive_quantity" in str(e):
                    restock_ids = check_restock(cursor)
                    insert_restock(cursor, restock_ids,
                                   supplier_ids, current_date)
                    continue

        cursor.execute(
            "update shop_order set status = 'Shipped' where id = %s", (order_id,))

        reviews = order[8]

        insert_order_reviews(cursor, reviews, created_at)

        i += 1
        if i % 50 == 0:
            restock_ids = check_restock(cursor)
            insert_restock(cursor, restock_ids, supplier_ids, current_date)

        if i % 100 == 0:
            print(f"{i}/{length} orders inserted")


def insert_order_reviews(cursor, reviews, created_at):
    for review in reviews:
        product_id = review[0]
        user_id = review[1]
        rating = review[2]
        description = review[3]

        cursor.execute("insert into review (product_id, user_id, rating, description, created_at) values (%s, %s, %s, %s, %s)",
                       (product_id, user_id, rating, description, created_at))


def run():

    conn = connect()

    users = generate_users()

    suppliers = generate_suppliers()

    with conn.cursor() as cursor:
        cursor.execute(
            "insert into role (role_name) values ('user') on conflict do nothing")
        cursor.execute(
            "insert into role (role_name) values ('admin') on conflict do nothing")
        cursor.execute(
            "insert into role (role_name) values ('employee') on conflict do nothing"
        )

        user_ids = insert_users(users, cursor)
        write_users(users)

        supplier_ids = insert_suppliers(suppliers, cursor)

        products = get_products(cursor)
        orders = generate_orders(user_ids, products)
        insert_orders(orders, cursor, supplier_ids)

        conn.commit()

    conn.close()
