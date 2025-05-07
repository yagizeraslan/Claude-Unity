# 📋 Changelog

All notable changes to **Claude API for Unity** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [1.0.0] - 2025-05-07

### 🎉 Added
- Support for Unity 2020.3, 2021, 2022, 2023, and 6.0+
- UPM (Unity Package Manager) Git installation support
- Support for Claude 3 family models (Opus, Sonnet, Haiku)
- Native SSE streaming support via Claude's `stream: true` API
- Runtime-safe API Key storage using `ClaudeSettings` ScriptableObject
- Full-text and streaming response modes via modular ClaudeChatController
- Sample Scene, prefabs, and demo UI included

### 🛠 Fixed
- Removed unnecessary dependency on Newtonsoft.Json
- Fixed potential streaming glitches and chunking errors using Unity-native handlers

---