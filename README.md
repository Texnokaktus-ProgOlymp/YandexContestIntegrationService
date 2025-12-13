# Yandex Contest Integration Service

## Configuration

### Yandex OAuth

The preferred approach to configure the Yandex OAuth client is by using the environment variables:

- `YandexAppParameters__ClientId` - the Client ID obtained provided by Yandex
- `YandexAppParameters__ClientSecret` - the Client Secret provided by Yandex

### S3

The preferred approach to configure the S3 client is by using the environment variables:

- `AWS__ServiceURL` - the URL of S3-compatible service
- `AWS_ACCESS_KEY_ID` - AWS Access Key
- `AWS_SECRET_ACCESS_KEY` - AWS Secret Key
