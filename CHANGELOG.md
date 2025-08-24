# 📋 Changelog

All notable changes to **Claude API for Unity** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [1.1.0] - 2025-08-24

### 🎉 Added
- **Claude 4 Family Support**: Added Claude 4.1 Opus, Claude 4 Opus, and Claude 4 Sonnet models
  - Claude 4.1 Opus: Most advanced model with enhanced coding and reasoning
  - Claude 4 Opus: Hybrid reasoning modes with instant and extended thinking
  - Claude 4 Sonnet: 1 million token context window support
- **Advanced Memory Management System**:
  - Configurable conversation history limits via `ClaudeSettings`
  - Automatic trimming of oldest messages to prevent unbounded memory growth
  - Default limit: 50 messages (configurable, 0 = unlimited)
- **Enhanced Resource Management**:
  - `IDisposable` implementation for `ClaudeChatController`
  - Proper event handler cleanup in `OnDestroy()`
  - Automatic controller disposal on GameObject destruction

### 🛠 Fixed
- **Critical Memory Leaks**:
  - Fixed `UnityWebRequest` disposal in streaming API
  - Fixed controller recreation on every message send
  - Fixed `TaskCompletionSource` leak in `UnityWebRequestAwaiter`
- **Performance Improvements**:
  - Replaced string concatenation with `StringBuilder` for streaming
  - Eliminated memory churn during token streaming (~80% reduction in allocations)
  - Improved overall memory efficiency for long-running conversations
- **Code Quality**:
  - Fixed namespace inconsistency (`DeepSeek` → `Claude`)
  - Added proper disposal patterns throughout the codebase
  - Enhanced error handling and resource cleanup

### 🔧 Changed
- Default model updated from `Claude_3_7_Sonnet` to `Claude_4_Sonnet` in sample
- Memory management settings now accessible via Unity Inspector
- Streaming implementation optimized for better performance

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