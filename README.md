# FaceCodex API 🛡️👤

**FaceCodex API** è un sistema di sicurezza biometrica di livello enterprise sviluppato in **C# / .NET**. L'applicazione fornisce un'infrastruttura completa per l'addestramento, il riconoscimento e l'identificazione facciale, integrando un modulo avanzato di cross-referencing con i database globali di pubblica sicurezza, tra cui il database dei ricercati dell'**FBI**.

---

## 📋 Indice
* [Descrizione del Progetto](#descrizione-del-progetto)
* [Caratteristiche Principali](#caratteristiche-principali)
* [Architettura](#architettura)
* [Tecnologie Utilizzate](#tecnologie-utilizzate)
* [Integrazione Forense](#integrazione-forense)

---

## 🚀 Descrizione del Progetto

FaceCodex è progettata per essere il cervello di sistemi di sorveglianza intelligenti e piattaforme di verifica dell'identità. L'API trasforma i dati visivi non strutturati in "Codici Facciali" (vettori matematici) che possono essere memorizzati, ricercati e confrontati in millisecondi.

Il cuore del sistema risiede nella sua capacità di apprendimento continuo: più immagini di un soggetto vengono fornite, più accurato diventa il riconoscimento, riducendo drasticamente i falsi positivi anche in condizioni di scarsa illuminazione o angolazioni parziali.

## ✨ Caratteristiche Principali

* **Neural Face Training:** Addestramento dinamico di modelli biometrici su dataset proprietari.
* **Identificazione Real-Time:** Riconoscimento istantaneo di individui all'interno di flussi video o immagini statiche.
* **Deep Search Comparison:** Algoritmo di confronto 1:N per identificare soggetti sconosciuti all'interno di un database di milioni di record.
* **FBI Most Wanted Integration:** Modulo di scansione automatica per il confronto dei tratti somatici con la lista dei ricercati dell'FBI (Criminal Investigative Division).
* **Liveness Detection:** Protezione contro attacchi di spoofing (tentativi di bypass tramite foto o video del volto).

## 🏗️ Architettura

L'API segue i principi della **Clean Architecture** ed è strutturata in vari layer:

1.  **Ingestion Layer:** Ricezione e normalizzazione delle immagini (correzione esposizione e allineamento pupillare).
2.  **Extraction Layer:** Utilizzo di reti neurali convoluzionali (CNN) per generare l'embedding facciale a 128 o 512 dimensioni.
3.  **Matching Engine:** Motore di ricerca vettoriale per il calcolo della distanza euclidea o similarità del coseno tra volti.
4.  **External Gateway:** Interfaccia di comunicazione sicura verso le API governative e database internazionali.

## 🛠️ Tecnologie Utilizzate

* **Runtime:** .NET 8.0 / C#
* **AI Engine:** DlibDotNet / Emgu CV (OpenCV per .NET)
* **Database Vettoriale:** PostgreSQL con estensione `pgvector`
* **Interfaccia:** ASP.NET Core Web API con supporto Swagger/OpenAPI
* **Sicurezza:** Integrità dei dati tramite hashing dei template biometrici

## 🔍 Integrazione Forense

FaceCodex include un connettore specializzato progettato per interfacciarsi con il database criminale dell'**FBI**. Questo modulo permette di:
* Sincronizzare periodicamente i profili dei ricercati ad alto rischio.
* Inviare alert automatici in caso di corrispondenza superiore alla soglia di confidenza del 95%.
* Generare report forensi dettagliati con metadati di confronto biometrico.

---

> **Nota Legale:** L'uso di FaceCodex API deve essere conforme alle normative locali sulla privacy (GDPR, CCPA) e ai protocolli di sicurezza nazionale. L'accesso ai dati FBI richiede credenziali istituzionali valide.

---
*Sviluppato con dedizione per un mondo più sicuro.*
