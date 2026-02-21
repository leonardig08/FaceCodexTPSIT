# FaceCodex API 👤🔐

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-9.0-brightgreen)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-orange)](https://swagger.io/)

**FaceCodex API** è un servizio Web REST sviluppato in **C# / .NET 9.0** per l’addestramento e il riconoscimento facciale.  
Il progetto integra servizi esterni di Face Recognition e permette la gestione sicura dei dati tramite **JWT Token**.

---

## 📋 Indice
- [Descrizione]()
- [Caratteristiche]()
- [Tecnologie]()
- [Autenticazione JWT]()
- [Installazione]()
- [Esempi di Endpoints]()

---

## 🚀 Descrizione

FaceCodex API consente di:

- Registrare persone reali tramite immagini
- Addestrare il sistema al riconoscimento del volto
- Riconoscere volti da nuove immagini
- Proteggere gli endpoint con **JWT**

Il sistema sfrutta **SkyBiometry** per il rilevamento e riconoscimento facciale e **ImgBB** per l’hosting delle immagini.

---

## ✨ Caratteristiche

- **Aggiunta Persona:** carica immagine → rileva volto → salva tag → addestra modello  
- **Riconoscimento Volti:** carica immagine → confronta con database → restituisce risultato  
- **JWT Authentication:** accesso protetto tramite token  
- **Swagger:** documentazione e test API integrati  

---

## 🛠️ Tecnologie

- **Runtime:** .NET 9.0 / C#  
- **Framework:** ASP.NET Core Web API  
- **Autenticazione:** JWT Bearer  
- **API Esterne:**  
  - [SkyBiometry](https://skybiometry.com/demo/face-recognition-demo/)  
  - [ImgBB](https://it.imgbb.com/)
- **Documentazione:** Swagger / OpenAPI  

---

## 🔐 Autenticazione JWT

1. Effettua login:  
   ```http
   POST /api/auth/login
   {
     "username": "admin",
     "password": "1234"
   }
   ```
2. Inserisci token nell’header Authorization per chiamare endpoint protetti:
    ```css
    Authorization: {token}
    ```

---

## ⚙️ Installazione

1. Clona il repository:
    ```bash
    git clone https://github.com/leonardig08/FaceCodexAPI.git
    ```
2. Apri il progetto in Visual Studio 2022
3. Configura ``` appsettings.json ``` con le credenziali SkyBiometry e ImgBB, oltre a JWT key/issuer/audience
4. Esegui il progetto (F5)
5. Accedi a Swagger:
    ```bash
    https://localhost:{porta}/swagger
    ```

---

## 📡 Esempi di Endpoints

1. **Autenticazione**
    ```http
    POST /api/auth/login
    ```

2. **Inserimento Persona**
    ```http
    POST /api/facecodex/AddPerson
    ```
   Body esempio:
    ```json
    {
      "nome": "Mario",
      "cognome": "Rossi",
      "imageUrl": "mario.jpg"
    }
    ```

3. **Riconoscimento Persona**
    ```http
    POST /api/facecodex/CheckPerson
    ```
   Body esempio:
    ```json
    {
      "imageUrl": "mario_test.jpg"
    }
    ```
  

  


