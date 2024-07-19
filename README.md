# DICOM Import & Edit

## Descrizione
**DICOM Import & Edit** è un'applicazione per la gestione, modifica e invio di file DICOM. Permette di importare file DICOM da diverse fonti, modificare l'ID del paziente e inviare i file al server PACS.

## Funzionalità
- Importazione di file DICOM da singoli file, cartelle e DICOMDIR.
- Modifica dell'ID del paziente nei file DICOM.
- Invio di file DICOM al server PACS.
- Gestione delle configurazioni del server PACS.
- Supporto per operazioni batch e gestione della coda di file.

## Requisiti di sistema
- Sistema operativo: Windows 10 o successivo
- .NET Framework 8.0
- Spazio su disco: minimo 100 MB
- RAM: minimo 4 GB

## Installazione
1. Scaricare l'installer di DICOM Import & Edit.
2. Eseguire il file `DCMImpSetup.exe` e seguire le istruzioni sullo schermo.
3. L'installer verificherà la presenza di .NET Framework 8.0 e, se necessario, lo scaricherà e lo installerà automaticamente.
4. Una volta completata l'installazione, avviare DICOM Import & Edit dal menu Start o dall'icona sul desktop.

## Utilizzo
1. **Importazione dei file DICOM**: Cliccare su uno dei pulsanti di importazione per importare singoli file, cartelle o DICOMDIR.
2. **Modifica dell'ID Paziente**: Selezionare uno o più file dalla tabella, inserire il nuovo ID del paziente e cliccare su "Modifica ID Paziente".
3. **Invio dei file al PACS**: Selezionare i file da inviare e cliccare su "Invia al PACS".
4. **Configurazione**: Accedere al menu delle impostazioni cliccando sull'icona dell'ingranaggio in basso a destra. Inserire i parametri di configurazione del server PACS e salvarli.


## Licenza
Distribuito sotto la licenza MIT. Vedi [LICENSE](./LICENSE) per i dettagli.
