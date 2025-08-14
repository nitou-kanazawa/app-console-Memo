# メモ帳アプリ開発仕様書

## プロジェクト概要
バックエンド実装の学習を目的とした、.NET コンソールアプリと REST API で連携するメモ帳アプリケーションの開発

## 学習目標
- **Entity Framework Core（ORM）** の理解と実装
- **ASP.NET Core Web API** の理解と実装
- 拡張性のあるアーキテクチャ設計の体験

## 技術スタック

### バックエンド
- **.NET 8** (最新LTS)
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQLite** (データベース)

### フロントエンド
- **.NET コンソールアプリケーション**
- **HttpClient** (API通信)

### 開発環境
- **Git** + **GitHub**
- **GitHub CLI** または ブラウザでのPR操作

## アーキテクチャ設計

### プロジェクト構成
```
MemoApp/
├── MemoApp.API/          # Web API プロジェクト
├── MemoApp.Core/         # ドメインモデル・インターフェース
├── MemoApp.Infrastructure/ # データアクセス・外部サービス
├── MemoApp.Console/      # コンソールアプリケーション
└── MemoApp.Tests/        # 単体テスト (将来拡張用)
```

### レイヤー構成 (Clean Architecture ベース)
1. **Domain Layer** (`MemoApp.Core`)
   - エンティティ
   - リポジトリインターフェース
   - ドメインサービス

2. **Infrastructure Layer** (`MemoApp.Infrastructure`)
   - Entity Framework Core 実装
   - リポジトリ実装
   - データベースコンテキスト

3. **API Layer** (`MemoApp.API`)
   - コントローラー
   - DTOs
   - 依存性注入設定

4. **Presentation Layer** (`MemoApp.Console`)
   - ユーザーインターフェース
   - API クライアント

## データモデル

### Memo エンティティ
```csharp
public class Memo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Tag> Tags { get; set; }
}
```

### Tag エンティティ
```csharp
public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Memo> Memos { get; set; }
}
```

### リレーション
- Memo と Tag: 多対多の関係

## API 設計

### エンドポイント一覧

#### メモ関連
- `GET /api/memos` - メモ一覧取得
- `GET /api/memos/{id}` - メモ詳細取得
- `POST /api/memos` - メモ作成
- `PUT /api/memos/{id}` - メモ更新
- `DELETE /api/memos/{id}` - メモ削除
- `GET /api/memos/search?query={searchTerm}` - メモ検索

#### タグ関連
- `GET /api/tags` - タグ一覧取得
- `POST /api/tags` - タグ作成
- `DELETE /api/tags/{id}` - タグ削除

### レスポンス形式
```json
{
  "success": true,
  "data": { ... },
  "message": "string"
}
```

## コンソールアプリ機能

### メニュー構成
1. メモ一覧表示
2. メモ詳細表示
3. メモ作成
4. メモ編集
5. メモ削除
6. メモ検索
7. タグ管理
8. 終了

### 操作フロー例
```
=== メモ帳アプリ ===
1. メモ一覧
2. メモ作成
3. メモ検索
4. タグ管理
5. 終了
選択してください: 1

=== メモ一覧 ===
[1] プロジェクト計画 (2024-08-14) [計画, 開発]
[2] 学習メモ (2024-08-13) [学習, .NET]
...
```

## 開発フロー

### Git ブランチ戦略
- **main**: 本番リリース用
- **develop**: 開発統合ブランチ
- **feature/**: 機能開発ブランチ

### タスク分割とコミット方針

#### フェーズ1: プロジェクト初期設定
1. `feature/setup-project-structure`
   - ソリューション・プロジェクト作成
   - NuGet パッケージインストール
   - 基本フォルダ構成作成

#### フェーズ2: ドメイン層実装
2. `feature/domain-models`
   - エンティティクラス作成
   - インターフェース定義

#### フェーズ3: インフラ層実装
3. `feature/entity-framework-setup`
   - DbContext 作成
   - 初期マイグレーション
4. `feature/repository-implementation`
   - リポジトリ実装
   - データアクセス層完成

#### フェーズ4: API層実装
5. `feature/web-api-controllers`
   - コントローラー作成
   - DTO 定義
6. `feature/dependency-injection`
   - DI コンテナ設定
   - サービス登録

#### フェーズ5: コンソールアプリ実装
7. `feature/console-app-structure`
   - メニュー構造作成
   - API クライアント基盤
8. `feature/memo-crud-operations`
   - メモ CRUD 操作実装
9. `feature/search-and-tag-features`
   - 検索・タグ機能実装

#### フェーズ6: 統合・テスト
10. `feature/integration-testing`
    - 動作確認
    - バグ修正

### コミットメッセージ規約
```
feat: 新機能追加
fix: バグ修正
docs: ドキュメント更新
style: コードスタイル修正
refactor: リファクタリング
test: テスト追加・修正
chore: その他の変更
```

## 拡張性への配慮

### 将来の機能追加対応
- **認証機能**: ASP.NET Core Identity 導入
- **カテゴリ機能**: Category エンティティ追加
- **ファイル添付**: File エンティティ・Azure Blob Storage 連携
- **通知機能**: SignalR 導入
- **Web UI**: Blazor Server/WASM 追加

### アーキテクチャ拡張ポイント
- **Repository パターン**: 新しいデータソース対応
- **Service 層**: ビジネスロジック分離
- **Middleware**: 横断的関心事（ログ、例外処理）
- **Configuration**: appsettings による設定管理

## 開発開始手順

### 前提条件
- .NET 8 SDK インストール済み
- Git インストール済み
- GitHub CLI インストール済み（または GitHub アカウント）

### 初期セットアップ
1. GitHub リポジトリ作成
2. ローカルリポジトリ初期化
3. develop ブランチ作成
4. 最初の feature ブランチ作成

### Claude Code での作業指示
各フェーズの feature ブランチで以下の手順を実行：
1. ブランチ作成・切り替え
2. 実装作業
3. テスト・動作確認
4. コミット・プッシュ
5. プルリクエスト作成
6. develop ブランチへマージ

## 学習ポイント

### Entity Framework Core
- Code First アプローチ
- マイグレーション管理
- LINQ クエリ
- リレーションシップ設定
- パフォーマンス最適化

### ASP.NET Core Web API
- コントローラーアクション設計
- モデルバインディング
- 依存性注入
- ミドルウェアパイプライン
- HTTP レスポンス処理

## 参考リンク

### 公式ドキュメント
- [ASP.NET Core Web API](https://docs.microsoft.com/aspnet/core/web-api/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [.NET アプリケーションアーキテクチャガイド](https://docs.microsoft.com/dotnet/architecture/)

### 学習リソース
- [Clean Architecture with .NET](https://github.com/jasontaylordev/CleanArchitecture)
- [EF Core チュートリアル](https://docs.microsoft.com/ef/core/get-started/overview/first-app)
- [Web API チュートリアル](https://docs.microsoft.com/aspnet/core/tutorials/first-web-api)

---

この仕様書に基づいて、Claude Code での段階的な開発を進めてください。各フェーズでの実装詳細や技術的な質問があれば、随時相談してください。