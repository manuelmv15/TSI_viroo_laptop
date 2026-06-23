# VIROO Studio Template / Assembly Sequence

This template simplifies the process of creating training modules focused on **Assembly / Disassembly** works in the context of **VIROO**.

It provides a set of tools based on custom **Unity Editor Windows / Inspectors**, **Visual Scripting subsystems**, and a collection of prepared **prefabs** to enable very quick and straightforward creation of interactive collaborative guided content.

---

## 🚀 How It Works

The main idea is to support the creation of a **"Sequence"** composed of **"Completable Items"**.  
In the **[Demo]** example scene, the sequence is stored in a component named **[Completable Sequence Data]** in the **"SEQUENCE Manager"** GameObject present in the Hierarchy.

We provide a set of **"Completable Systems"** already implemented, but the system is designed to be **extended** with your own completion systems — which can be implemented in **Visual Scripting** or **C#**.

---

## 🧩 Completable Item Types

Those **"Completable Items"** can be of several types, including:

- **Audio Completables:** Wait for an audio clip to be played.  
- **Video Completables:** Wait for a video to finish playing.  
- **Playable Director Completables:** Wait for a Playable Director animation to complete.  
- **Trigger Completables:** Enable triggers activated by the "Player" or other defined "Tools".  
- **Assembly Target Completables:** Allow "Assembly Parts" to be placed on "Assembly Targets" configured with several parameters.  
- **Click Completables:** Wait for a user click — can be triggered by a regular mouse or an XR controller.

---

## ⚡ Quick Start

We recommend the following steps to get started quickly:

1. **Run the Demo Scene:**  
   Open **`Assets/Scenes/Demo`** and explore the self-explanatory setup.  
2. **Watch the Tutorials:**  
   Have a look at our **videos and webinars** associated with this template.  
3. **Explore Completion Systems:**  
   Check out the **Completion Systems** implemented in **Visual Scripting**.

---

## 🛠️ Assembly Parts Editor

To create, edit, and configure **Assembly Parts** and **Assembly Targets**, we provide a custom editor:

> **Menu Path:** `Window → VIROO → Assembly Templates → Assembly Parts Editor`

<p align="center">
  <img width="800" alt="Assembly Parts Editor" src="https://github.com/user-attachments/assets/d4f32757-bf0e-40de-9fe8-e5c3a23bbe04" />
</p>

---

## 💡 Extending the Template

The full code of the **Editor** and **Runtime Template** is included in the project and can be modified freely to add extra functionality.

If you find something that could improve the template, feel free to **contact us** or submit a **pull request**.

---

## ❤️ We Hope You Enjoy the Template!

