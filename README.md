# Toko Sederhana

REST API untuk manajemen toko online yang mencakup operasi CRUD pada data pelanggan, produk, dan pesanan.

- **Nama** : Ridho Rizky Prasetyo
- **NIM** : 242410102055
- **Kelas** : A
- **Mata Kuliah** : Pemrograman Antarmuka Aplikasi

---

## Teknologi

| Komponen | Detail |
|----------|--------|
| Bahasa | C# |
| Framework | ASP.NET Core Web API (.NET 8) |
| Database | PostgreSQL |
| Library | Npgsql |
| Dokumentasi API | Swagger |

---

## Struktur Folder

```
TugasLkm1/
├── Controllers/
│   ├── CustomersController.cs
│   ├── OrdersController.cs
│   └── ProductsController.cs
├── Helper/
│   └── DBHelper.cs
├── Models/
│   ├── Customer.cs
│   ├── Order.cs
│   ├── Product.cs
│   └── Request.cs
├── Repositories/
│   ├── CustomerRepository.cs
│   ├── OrderRepository.cs
│   └── ProductRepository.cs
├── appsettings.json
├── Program.cs
└── toko_lkm1.sql
```

---

## Instalasi & Menjalankan Project

### 1. Clone repository

```bash
git clone https://github.com/Ridho-30/TugasLkm1.git
cd TugasLkm1
```

### 2. Install dependency

```bash
dotnet add package Npgsql
```

### 3. Sesuaikan connection string

Edit file `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost; Port=5432; Database=toko_lkm1; Username=postgres; Password=[password anda]"
  }
}
```

### 4. Import database

**Via psql:**
```bash
psql -U postgres -f toko_lkm1.sql
```

**Via pgAdmin:**
Buka pgAdmin → pilih server → klik kanan database → Query Tool → buka file `toko_lkm1.sql` → jalankan.

### 5. Jalankan project

```bash
dotnet run
```

Buka Swagger UI di browser:
```
https://localhost:{port}/swagger
```

---

## Desain Database

### Relasi Tabel

```
customers (id) ──< orders.customer_id
products  (id) ──< orders.product_id
```

### Tabel: customers

| Kolom | Tipe | Keterangan |
|-------|------|------------|
| id | SERIAL | Primary key |
| name | VARCHAR(100) | Nama pelanggan |
| email | VARCHAR(100) | Email unik |
| phone | VARCHAR(20) | Nomor telepon |
| address | TEXT | Alamat |
| created_at | TIMESTAMP | Waktu dibuat |
| updated_at | TIMESTAMP | Waktu diperbarui |

### Tabel: products

| Kolom | Tipe | Keterangan |
|-------|------|------------|
| id | SERIAL | Primary key |
| name | VARCHAR(150) | Nama produk |
| description | TEXT | Deskripsi produk |
| price | DECIMAL(12,2) | Harga |
| stock | INT | Stok |
| category | VARCHAR(50) | Kategori |
| is_deleted | SMALLINT | Soft delete (0=aktif, 1=terhapus) |
| created_at | TIMESTAMP | Waktu dibuat |
| updated_at | TIMESTAMP | Waktu diperbarui |

### Tabel: orders

| Kolom | Tipe | Keterangan |
|-------|------|------------|
| id | SERIAL | Primary key |
| customer_id | INT | FK ke customers |
| product_id | INT | FK ke products |
| quantity | INT | Jumlah pesanan |
| total_price | DECIMAL(12,2) | Total harga otomatis |
| status | VARCHAR(20) | pending / paid / shipped |
| created_at | TIMESTAMP | Waktu dibuat |
| updated_at | TIMESTAMP | Waktu diperbarui |

---

## Daftar Endpoint

### Customers

| Method | URL | Keterangan | Status Code |
|--------|-----|------------|-------------|
| GET | /api/Customers | Ambil semua customer | 200 |
| GET | /api/Customers/{id} | Ambil customer by ID | 200 / 404 |
| POST | /api/Customers | Tambah customer baru | 201 |
| PUT | /api/Customers/{id} | Update customer | 200 / 404 |
| DELETE | /api/Customers/{id} | Hapus customer | 200 / 404 |

### Products

| Method | URL | Keterangan | Status Code |
|--------|-----|------------|-------------|
| GET | /api/Products | Ambil semua produk aktif | 200 |
| GET | /api/Products/{id} | Ambil produk by ID | 200 / 404 |
| POST | /api/Products | Tambah produk baru | 201 |
| PUT | /api/Products/{id} | Update produk | 200 / 404 |
| DELETE | /api/Products/{id} | Soft delete produk | 200 / 404 |

### Orders

| Method | URL | Keterangan | Status Code |
|--------|-----|------------|-------------|
| GET | /api/Orders | Ambil semua order | 200 |
| GET | /api/Orders/{id} | Ambil order by ID | 200 / 404 |
| POST | /api/Orders | Buat order baru | 201 |
| PUT | /api/Orders/{id} | Update order | 200 / 404 |
| DELETE | /api/Orders/{id} | Hapus order | 200 / 404 |

---

## Format Response

### Sukses (data tunggal)
```json
{
  "status": "success",
  "data": { }
}
```

### Sukses (data list)
```json
{
  "status": "success",
  "meta": { "total": 5 },
  "data": [ ]
}
```

### Error
```json
{
  "status": "error",
  "message": "Pesan error yang informatif"
}
```

---

## Video Presentasi
```
{
  youtube.xxxxxxx
}
```
