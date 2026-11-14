# PBL3 Shop

Ứng dụng web bán hàng (đồ án PBL3): giao diện Vue 3 và Web API ASP.NET Core. Repo là monorepo gộp từ `PBL3-FE` và `pbl3` (giữ nguyên lịch sử commit). Lịch sử cũ của nhánh `main` backend được lưu ở nhánh `backup/main-before-monorepo`.

```text
pbl3/
├── backend/                  # ASP.NET Core Web API (.NET 8)
│   ├── Controllers/          # Auth, Brands, Carts, Categories, Orders, Products, Review, Statitics, Users
│   ├── Entities/             # Model EF Core + ShopPbl3Context
│   ├── DTO/
│   ├── Repository/
│   ├── Service/
│   ├── Properties/           # launchSettings.json
│   ├── Program.cs
│   ├── appsettings.json
│   └── pbl3-BE.csproj
├── frontend/                 # Vue 3 + Vite + Tailwind CSS
│   ├── index.html
│   ├── package.json
│   └── src/
│       ├── views/            # client/, admin/
│       ├── components/  layouts/  router/
│       ├── stores/           # Pinia
│       ├── services/  composables/  constants/  assets/
│       └── App.vue, main.js
├── .gitignore
└── README.md
```

## Công nghệ

| Phần | Công nghệ |
|------|-----------|
| Backend | C#, ASP.NET Core 8, Entity Framework Core 8, SQL Server, JWT Bearer, BCrypt, ClosedXML (xuất Excel), QuestPDF (xuất PDF), Swagger |
| Frontend | Vue 3, Vite 7, Vue Router, Pinia, Axios, Tailwind CSS 4 |

## Tính năng

- Đăng ký, đăng nhập JWT, quản lý người dùng.
- Sản phẩm, danh mục, thương hiệu, tải ảnh sản phẩm.
- Giỏ hàng, đặt hàng, quản lý đơn hàng.
- Đánh giá sản phẩm.
- Thống kê cho admin, xuất báo cáo Excel/PDF.
- Giao diện riêng cho khách hàng (`views/client`) và quản trị (`views/admin`).

## Chạy local

Yêu cầu: .NET SDK 8, SQL Server (hoặc SQL Express), Node.js 20+.

### Backend

```bash
cd backend
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "<chuỗi bí mật dài ít nhất 32 ký tự>"
# Thêm ConnectionStrings:DefaultConnection (SQL Server) vào user-secrets hoặc appsettings.Local.json
dotnet run          # mặc định https://localhost:7010
```

Swagger: `https://localhost:7010/swagger`.

### Frontend

```bash
cd frontend
npm install
npm run dev
npm run build
```

> Địa chỉ API đang ghi cứng `https://localhost:7010` trong `frontend/src` (ví dụ `stores/cart.js`, các view sản phẩm). Hãy đổi khi triển khai.

## Bảo mật

`Jwt:Key` trong `appsettings.json` chỉ là giá trị giữ chỗ. Dùng user-secrets hoặc biến môi trường `Jwt__Key`; không commit secret thật.
