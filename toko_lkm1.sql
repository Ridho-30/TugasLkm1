select * FROM customers
select * FROM products
select * FROM orders

CREATE TABLE customers (
    id          SERIAL PRIMARY KEY,
    name        VARCHAR(100) NOT NULL,
    email       VARCHAR(100) NOT NULL UNIQUE,
    phone       VARCHAR(20),
    address     TEXT,
    created_at  TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE products (
    id          SERIAL PRIMARY KEY,
    name        VARCHAR(150) NOT NULL,
    description TEXT,
    price       DECIMAL(12,2) NOT NULL,
    stock       INT NOT NULL DEFAULT 0,
    category    VARCHAR(50),
	is_deleted  SMALLINT NOT NULL DEFAULT 0,
    created_at  TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE orders (
    id           SERIAL PRIMARY KEY,
    customer_id  INT NOT NULL REFERENCES customers(id),
    product_id   INT NOT NULL REFERENCES products(id),
    quantity     INT NOT NULL,
    total_price  DECIMAL(12,2) NOT NULL,
    status       VARCHAR(20) NOT NULL DEFAULT 'pending',
    created_at   TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at   TIMESTAMP NOT NULL DEFAULT NOW()
);
CREATE INDEX idx_orders_customer_id ON orders(customer_id);
CREATE INDEX idx_orders_status ON orders(status);

INSERT INTO customers (name, email, phone, address) VALUES
('Ridhok Rizky',   'ridhok@email.com',   '081234567890', 'Jl. Tidar No.1, Surabaya'),
('Siti Rahayu',    'siti@email.com',   '082345678901', 'Jl. Melati No.2, Malang'),
('Ahmad Fauzi',    'ahmad@email.com',  '083456789012', 'Jl. Kenanga No.3, Jember'),
('Dewi Lestari',   'dewi@email.com',   '084567890123', 'Jl. Anggrek No.4, Bali'),
('Rizky Pratama',  'rizky@email.com',  '085678901234', 'Jl. Dahlia No.5, Jakarta');

INSERT INTO products (name, description, price, stock, category) VALUES
('Laptop Asus A416',   'Laptop 14 inch RAM 8GB',      7500000.00, 20, 'Elektronik'),
('Mouse Logitech M100','Mouse USB wired',               150000.00, 50, 'Aksesoris'),
('Keyboard Rexus',     'Keyboard mechanical TKL',       450000.00, 30, 'Aksesoris'),
('Monitor LG 24 inch', 'Monitor FHD IPS 75Hz',        2800000.00, 15, 'Elektronik'),
('Headset JBL T110',   'In-ear wired headset',          200000.00, 40, 'Aksesoris');

INSERT INTO orders (customer_id, product_id, quantity, total_price, status) VALUES
(1, 1, 1, 7500000.00, 'paid'),
(2, 2, 2,  300000.00, 'pending'),
(3, 3, 1,  450000.00, 'paid'),
(4, 4, 1, 2800000.00, 'shipped'),
(5, 5, 3,  600000.00, 'pending');