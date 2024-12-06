# LnAddress.Net

**LnAddress.Net** is a service that allows you to receive Lightning payments using any username at your domain. For
example: `username@your.domain`.

## Overview

- **Docker Image**: A pre-built Docker image is available
  at [ipmsio/lnaddress.net](https://hub.docker.com/r/ipmsio/lnaddress.net).
- **Configuration Reference**: Review the [docker-compose.yml](docker-compose.yml) file for a complete list of
  environment variables and configuration options.
- **Reverse Proxy Setup**: An example Nginx configuration is provided in [example.nginx](example.nginx).

## Getting Started

1. **Pull the Docker Image**:

   ```bash
   docker pull ipmsio/lnaddress.net:latest
   ```

2. **Review Configuration Variables**:

   Check the [docker-compose.yml](docker-compose.yml) file for environment variables. These variables allow you to:

    - Configure connection details to your LND instance.
    - Adjust limits for payment amounts or comment fields.

3. **Set Up Nginx (Optional)**:

   For a production setup, use [example.nginx](example.nginx) as a guide to set up a reverse proxy with TLS termination.

## Connecting to LND

To enable Lightning payments, you need to connect LnAddress.Net to your LND instance. You will need:

- The **TLS certificate** (`tls.cert`)
- The **admin.macaroon** in base64 format
- The **LND RPC server endpoint**

**Steps to Obtain LND Credentials**:

1. **TLS Certificate**:

   Extract the certificate content between the `-----BEGIN CERTIFICATE-----` and `-----END CERTIFICATE-----` lines.

   ```bash
   cat /.lnd/tls.cert
   ```

   Copy only the certificate portion without the header and footer lines.

2. **Invoice Macaroon**:

   Convert the `invoice.macaroon` to a single-line base64 string:

   ```bash
   base64 /.lnd/data/chain/bitcoin/mainnet/invoice.macaroon | tr -d '\n'
   ```

3. **RPC Server URL**:

   Set your LND RPC endpoint, for example:

   ```bash
   https://<lnd-ip>:10009 
   ```

   Ensure your `lnd.conf` includes:

   ```ini
   rpclisten=0.0.0.0:10009
   ```

   This makes LND’s RPC interface accessible to LnAddress.Net.

## Default Settings

- **MinSendable**: 1,000 millisatoshis (1 satoshi)
- **MaxSendable**: 100,000,000 millisatoshis (100,000 satoshis)
- **MaxCommentAllowed**: 0 (no comments accepted)

If these defaults don’t meet your needs, adjust them via environment variables as shown
in [docker-compose.yml](docker-compose.yml).

## Running the Service

Once you have your environment variables set and Docker is ready, you can run:

```bash
docker-compose up -d
```

or, if running standalone:

```bash
docker run -d \
  -p 80:80 \
  -e LND__CERT="<base64_tls_cert>" \
  -e LND__MACAROON="<base64_admin_macaroon>" \
  -e LND__RPCADDRESS="https://<lnd-ip>:10009" \
  ipmsio/lnaddress.net:latest
```

Replace the environment variables with your actual values.

---

**You’re now set up to receive Lightning payments via username addresses on your domain!**