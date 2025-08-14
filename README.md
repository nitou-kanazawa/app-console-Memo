# メモ帳アプリケーション

バックエンド実装の学習を目的とした、.NET コンソールアプリと REST API で連携するメモ帳アプリケーション

## 🎯 学習目標

- **Entity Framework Core（ORM）** の理解と実装
- **ASP.NET Core Web API** の理解と実装  
- 拡張性のあるアーキテクチャ設計の体験

## 🏗️ アーキテクチャ

### Clean Architecture 実装
```
MemoApp/
├── MemoApp.API/          # Web API プロジェクト
├── MemoApp.Core/         # ドメインモデル・インターフェース
├── MemoApp.Infrastructure/ # データアクセス・外部サービス
├── MemoApp.Console/      # コンソールアプリケーション
└── MemoApp.Tests/        # 単体テスト (将来拡張用)
```

### 技術スタック
- **.NET 8** (最新LTS)
- **ASP.NET Core Web API**
- **Entity Framework Core** + **SQLite**
- **HttpClient** (API通信)

## 🚀 使用方法

### 1. API サーバー起動
```bash
cd MemoApp.API
dotnet run --urls http://localhost:5000
```

### 2. コンソールアプリ実行
```bash
cd MemoApp.Console
dotnet run
```

## 📱 機能一覧

### REST API エンドポイント
- `GET /api/memos` - メモ一覧取得
- `GET /api/memos/{id}` - メモ詳細取得
- `POST /api/memos` - メモ作成
- `PUT /api/memos/{id}` - メモ更新
- `DELETE /api/memos/{id}` - メモ削除
- `GET /api/memos/search?query={searchTerm}` - メモ検索
- `GET /api/tags` - タグ一覧取得
- `POST /api/tags` - タグ作成
- `DELETE /api/tags/{id}` - タグ削除

### コンソールアプリメニュー
1. **メモ一覧表示** - タグ付きでメモ一覧を表示
2. **メモ詳細表示** - 指定したメモの詳細を表示
3. **メモ作成** - 新しいメモをタグ付きで作成
4. **メモ編集** - 既存メモのタイトル・内容・タグを編集
5. **メモ削除** - 確認後にメモを削除
6. **メモ検索** - タイトル・内容・タグで検索
7. **タグ管理** - タグの一覧・作成・削除
8. **終了** - アプリケーション終了

## 🛠️ 開発・ビルド

### 前提条件
- .NET 8 SDK
- Git

### ビルド
```bash
dotnet build
```

### データベース初期化
```bash
cd MemoApp.API
dotnet ef database update
```

## 📁 プロジェクト構成

### MemoApp.Core (Domain Layer)
- `Entities/` - エンティティクラス (Memo, Tag)
- `Interfaces/` - リポジトリインターフェース

### MemoApp.Infrastructure (Infrastructure Layer)
- `Data/` - Entity Framework DbContext
- `Repositories/` - リポジトリ実装
- `Migrations/` - データベースマイグレーション

### MemoApp.API (API Layer)
- `Controllers/` - Web API コントローラー
- `DTOs/` - データ転送オブジェクト

### MemoApp.Console (Presentation Layer)
- `Services/` - API クライアントサービス
- `UI/` - メニューシステム
- `Models/` - DTO モデル

## 🎨 主な実装パターン

- **Repository パターン** - データアクセスの抽象化
- **Dependency Injection** - 疎結合な設計
- **Clean Architecture** - レイヤー分離
- **非同期パターン** - async/await の活用
- **統一レスポンス形式** - API レスポンスの標準化

## 📚 学習ポイント

### Entity Framework Core
- Code First アプローチ
- マイグレーション管理
- LINQ クエリ
- リレーションシップ設定（多対多）
- パフォーマンス最適化（Include）

### ASP.NET Core Web API
- コントローラーアクション設計
- モデルバインディング・バリデーション
- 依存性注入
- エラーハンドリング
- HTTP ステータスコード対応

---

🤖 Generated with [Claude Code](https://claude.ai/code)