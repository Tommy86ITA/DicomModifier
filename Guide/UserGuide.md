# Guida per l'Utente di DICOM Modifier

## Panoramica dell'Interfaccia Utente

L'interfaccia utente di DICOM Modifier è progettata per essere intuitiva e facile da usare. Di seguito viene fornita una descrizione delle principali sezioni dell'interfaccia e delle loro funzionalità.

![Interfaccia Utente](./immagine.png)

1. **Selezione file**: Questa sezione consente di importare file DICOM, DICOMDIR o intere cartelle contenenti file DICOM.
   - **Apri file DICOM**: Consente di selezionare e importare un singolo file DICOM.
   - **Apri file DICOMDIR**: Consente di selezionare e importare un file DICOMDIR.
   - **Apri cartella**: Consente di selezionare e importare tutti i file DICOM contenuti in una cartella.

2. **Tabella degli esami**: Visualizza le informazioni sugli esami DICOM importati, tra cui:
   - Cognome e nome del paziente
   - Data di nascita
   - ID Paziente
   - Descrizione dello studio
   - Data dell'esame
   - Modalità
   - Serie
   - Immagini

3. **Nuovo ID Paziente (opzionale)**: Questa sezione permette di inserire un nuovo ID paziente per i file selezionati nella tabella.
   - **Inserire qui il nuovo ID**: Campo di testo per inserire il nuovo ID paziente.
   - **Modifica ID Paziente**: Pulsante per applicare il nuovo ID paziente ai file selezionati.

4. **Invio**: Questa sezione consente di inviare i file DICOM selezionati al server PACS.
   - **Invia al PACS**: Pulsante per avviare l'invio dei file DICOM al server PACS.

5. **Pulisci**: Questo pulsante permette di cancellare la coda dei file DICOM.

6. **Menu delle impostazioni**: Contiene opzioni aggiuntive e informazioni sul programma.
   - **Esci**: Chiude il programma.
   - **About**: Mostra le informazioni sul programma e sullo sviluppatore.
   - **Impostazioni**: Apre la finestra delle impostazioni per configurare i parametri del server PACS.

7. **Barra di stato**: Mostra lo stato corrente del programma e il numero di file importati.

## Come Utilizzare DICOM Modifier

1. **Importazione dei file DICOM**
   - Clicca su "Apri file DICOM" per importare un singolo file DICOM.
   - Clicca su "Apri file DICOMDIR" per importare un file DICOMDIR.
   - Clicca su "Apri cartella" per importare tutti i file DICOM contenuti in una cartella.

2. **Modifica dell'ID Paziente**
   - Seleziona uno o più esami nella tabella degli esami.
   - Inserisci il nuovo ID paziente nel campo di testo "Inserire qui il nuovo ID".
   - Clicca su "Modifica ID Paziente" per applicare il nuovo ID paziente ai file selezionati.

3. **Invio dei file DICOM al server PACS**
   - Seleziona uno o più esami nella tabella degli esami.
   - Clicca su "Invia al PACS" per avviare l'invio dei file DICOM al server PACS.

4. **Impostazioni del programma**
   - Clicca su "Impostazioni" nel menu delle impostazioni per aprire la finestra delle impostazioni.
   - Configura i parametri del server PACS (IP, porta, AE Title, timeout, AE Title locale).
   - Salva le impostazioni cliccando su "Salva".

5. **Informazioni sul programma**
   - Clicca su "About" nel menu delle impostazioni per visualizzare le informazioni sul programma e sullo sviluppatore.

6. **Uscita dal programma**
   - Clicca su "Esci" nel menu delle impostazioni per chiudere il programma.

## Note
- Assicurati di configurare correttamente i parametri del server PACS nelle impostazioni prima di inviare i file.
- Dopo aver modificato l'ID paziente, verifica che i file siano stati aggiornati correttamente nella tabella degli esami.

Questa guida copre le funzionalità principali di DICOM Modifier e dovrebbe aiutarti a utilizzare il programma in modo efficace. Se hai ulteriori domande, non esitare a contattare il supporto tecnico.
