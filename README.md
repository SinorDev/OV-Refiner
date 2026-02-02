# 🌐 OV Refiner - Domain to IP Resolver for OVPN & V2Ray Configs

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%208.0-purple)
![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%208.0-512BD4?style=flat&logo=dotnet)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?style=flat&logo=windows)
![Language](https://img.shields.io/badge/Language-C%23-239120?style=flat&logo=csharp)
![License](https://img.shields.io/badge/License-GPLv3-blue.svg)


**OV Refiner** is a specialized tool developed in C# that resolves domain endpoints via **DoH**, and generates **IP-based V2Ray & OVPN configuration files** to bypass **DNS Hijacking**, **DNS Spoofing** or **IP Poisoning**.

## Technical Overview

The application functions as a configuration parser and DNS resolver wrapper under  GPL-3.0 license.

## 🚀 Features

- **Multi-Protocol Support:** Handles `vmess://`, `vless://`, `trojan://`, `ss://`, and `.ovpn` files.
- **Secure DNS Resolution:**
  - Built-in support for **Google DNS** and **Cloudflare**.
  - **Custom DoH Support:** Ability to input custom DoH URLs.
  - **Binary DNS Request (RFC 8484):** Optional Base64 URL encoding for advanced firewall evasion.
  * **Comprehensive IP Resolution:** Identifies and extracts *all* available IP addresses (A-records) behind a single domain, generating separate configurations for each discovered endpoint.
- **Smart Parsing:**
  - Automatically ignores configurations that are already using IP addresses.
  - Decodes and re-encodes Base64 Vmess configs correctly using JSON parsing.
  - Handles URL Encoding for clean config names.
- **Organization & Naming:**
  - Appends `| IP {n}` to config names for easy identification.
  - Creates organized output folders/files.
- **Modern UI:** Dark-themed Windows Forms interface with logs.

## Installation & Build

### Prerequisites
* Visual Studio 2022 (v17.0 or later)
* .NET 6.0 SDK (or .NET 8.0)
* NuGet Package Manager

### Building from Source

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/SinorDev/OV-Refiner.git](https://github.com/SinorDev/OV-Refiner.git)
    cd OV-Refiner
    ```

2.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```

3.  **Build the project:**
    ```bash
    dotnet build --configuration Release
    ```
Or open `OVRefiner.sln` in Visual Studio to Build and Run.

## Usage

1.  Launch the executable from `bin/Release/net6.0-windows/OV-Refiner.exe`.
2.  **Select Config Type:** Choose between V2Ray protocols or OpenVPN.
3.  **Input Source:**
    * *V2Ray:* Select a text file containing configuration strings (one per line) or Base64 subscription text.
    * *OpenVPN:* Select a directory containing `.ovpn` files.
4.  **DNS Configuration:** Select a provider or input a custom DoH endpoint.
    * *Optional:* Enable "Binary Request" for custom DoH servers requiring RFC 8484 payloads.
5.  **Start:** Press **START PROCESS**.
6.  **Output:** Processed files are generated in the source directory with timestamped filenames (`yyyyMMdd-mmhhss`).

---

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE) file for details.
