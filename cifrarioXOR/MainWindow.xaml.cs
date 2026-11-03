
using System;
using System.Windows;

namespace cifrarioXOR
{
    public partial class MainWindow : Window
    {
        // CHIAVE XOR
        private string chiave = "ABC";

        // MODALITA: false = XOR, true = RSA
        private bool modalitaRSA = false;

        // NUMERI PRIMI UTILIZZATI PER RSA
        private int p = 17;
        private int q = 5;

        // VALORI CALCOLATI PER RSA
        private int N;
        private int V;

        // ESPONENTI DELLE CHIAVI RSA
        private int Npub;
        private int Npri = 5;


        // COSTRUTTORE DELLA FINESTRA
        public MainWindow()
        {
            InitializeComponent();

            // CALCOLO DI N
            // N = p * q
            N = p * q;

            // CALCOLO DI V
            // V = (p - 1) * (q - 1)
            V = (p - 1) * (q - 1);

            // CALCOLO DI Npub
            // Cerchiamo un numero che soddisfi:
            // (Npub * Npri) mod V = 1
            Npub = 1;

            while ((Npub * Npri) % V != 1)
            {
                Npub++;
            }
        }


        // CAMBIA TRA XOR E RSA
        private void btnCambia_Click(object sender, RoutedEventArgs e)
        {
            // Se siamo in modalità XOR, passiamo a RSA
            if (modalitaRSA == false)
            {
                modalitaRSA = true;
                btnCambia.Content = "Passa a XOR";
            }
            else
            {
                // Altrimenti torniamo a XOR
                modalitaRSA = false;
                btnCambia.Content = "Passa a RSA";
            }

            // Cancelliamo i risultati precedenti
            txtCifrato.Text = "";
            txtDecifrato.Text = "";
        }


        // APPLICA L'ALGORITMO XOR
        private int[] ApplicaXOR(int[] dati)
        {
            // Creiamo un array per i risultati
            int[] risultato = new int[dati.Length];

            // Scorriamo tutti i caratteri del messaggio
            for (int i = 0; i < dati.Length; i++)
            {
                // Prendiamo il carattere corrispondente della chiave
                // L'operatore % permette di ripetere la chiave
                int carattereChiave = chiave[i % chiave.Length];

                // Eseguiamo l'operazione XOR
                risultato[i] = dati[i] ^ carattereChiave;
            }

            return risultato;
        }


        // CALCOLA UNA POTENZA MODULARE
        private int PotenzaModulo(int baseNumero, int esponente, int modulo)
        {
            // Inizializziamo il risultato a 1
            int risultato = 1;

            // Moltiplichiamo la base per se stessa
            // per il numero di volte indicato dall'esponente
            for (int i = 0; i < esponente; i++)
            {
                // Dopo ogni moltiplicazione calcoliamo il modulo
                risultato = (risultato * baseNumero) % modulo;
            }

            return risultato;
        }


        // LEGGE I NUMERI SEPARATI DA SPAZI
        private int[] LeggiNumeri(string testo)
        {
            int quantita = 0;
            bool dentroNumero = false;

            // CONTIAMO QUANTI NUMERI SONO PRESENTI
            for (int i = 0; i < testo.Length; i++)
            {
                char carattere = testo[i];

                // Controlliamo se il carattere è una cifra
                if (carattere >= '0' && carattere <= '9')
                {
                    // Se prima non eravamo dentro un numero,
                    // abbiamo trovato un nuovo numero
                    if (dentroNumero == false)
                    {
                        quantita++;
                    }

                    dentroNumero = true;
                }
                // Riconosciamo gli spazi e gli altri separatori
                else if (carattere == ' ' || carattere == '\n' ||
                         carattere == '\r' || carattere == '\t')
                {
                    dentroNumero = false;
                }
                else
                {
                    // Se troviamo un carattere non numerico,
                    // il testo non è valido
                    throw new Exception("Carattere non valido!");
                }
            }

            // Creiamo un array della dimensione necessaria
            int[] numeri = new int[quantita];

            int posizione = 0;
            int numero = 0;
            dentroNumero = false;

            // LEGGIAMO I NUMERI E LI INSERIAMO NELL'ARRAY
            for (int i = 0; i < testo.Length; i++)
            {
                char carattere = testo[i];

                if (carattere >= '0' && carattere <= '9')
                {
                    // Costruiamo il numero cifra per cifra
                    numero = numero * 10 + (carattere - '0');
                    dentroNumero = true;
                }
                else
                {
                    // Quando troviamo un separatore,
                    // salviamo il numero appena letto
                    if (dentroNumero == true)
                    {
                        numeri[posizione] = numero;

                        posizione++;
                        numero = 0;
                        dentroNumero = false;
                    }
                }
            }

            // Salviamo anche l'ultimo numero, se presente
            if (dentroNumero == true)
            {
                numeri[posizione] = numero;
            }

            return numeri;
        }


        // PULSANTE CRITTOGRAFA
        private void btnCrittografa_Click(object sender, RoutedEventArgs e)
        {
            // Leggiamo il messaggio inserito
            string messaggio = txtMessaggio.Text;

            // Controlliamo che il messaggio non sia vuoto
            if (messaggio.Length == 0)
            {
                MessageBox.Show("Inserisci un messaggio!");
                return;
            }

            // Trasformiamo ogni carattere nel suo valore numerico
            int[] dati = new int[messaggio.Length];

            for (int i = 0; i < messaggio.Length; i++)
            {
                dati[i] = messaggio[i];
            }

            // Array che conterrà il risultato della crittografia
            int[] risultato;

            if (modalitaRSA == false)
            {
                // CRITTOGRAFIA XOR
                risultato = ApplicaXOR(dati);
            }
            else
            {
                // CRITTOGRAFIA RSA

                // Controlliamo che ogni carattere sia minore di N
                for (int i = 0; i < dati.Length; i++)
                {
                    if (dati[i] >= N)
                    {
                        MessageBox.Show(
                            "RSA accetta solo caratteri con valore inferiore a " + N + "!"
                        );
                        return;
                    }
                }

                // Creiamo l'array per i risultati
                risultato = new int[dati.Length];

                // Cifriamo ogni carattere con la formula:
                // c = m^Npub mod N
                for (int i = 0; i < dati.Length; i++)
                {
                    risultato[i] = PotenzaModulo(dati[i], Npub, N);
                }
            }

            // Trasformiamo i risultati in una sequenza di numeri
            string testoCifrato = "";

            for (int i = 0; i < risultato.Length; i++)
            {
                testoCifrato += risultato[i] + " ";
            }

            // Mostriamo il messaggio crittografato
            txtCifrato.Text = testoCifrato;
        }


        // PULSANTE DECRITTOGRAFA
        private void btnDecrittografa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Leggiamo il messaggio crittografato
                string testo = txtCifrato.Text;

                // Controlliamo che non sia vuoto
                if (testo.Length == 0)
                {
                    MessageBox.Show("Prima devi crittografare un messaggio!");
                    return;
                }

                // Trasformiamo il testo in un array di numeri
                int[] dati = LeggiNumeri(testo);

                // Array che conterrà il risultato della decrittografia
                int[] risultato;

                if (modalitaRSA == false)
                {
                    // DECRITTOGRAFIA XOR
                    // Applichiamo nuovamente XOR con la stessa chiave
                    risultato = ApplicaXOR(dati);
                }
                else
                {
                    // DECRITTOGRAFIA RSA
                    risultato = new int[dati.Length];

                    // Decrittografiamo ogni numero
                    for (int i = 0; i < dati.Length; i++)
                    {
                        // Controlliamo che il numero sia compreso tra 0 e N - 1
                        if (dati[i] < 0 || dati[i] >= N)
                        {
                            throw new Exception("Numero RSA non valido!");
                        }

                        // Applichiamo la formula:
                        // m = c^Npri mod N
                        risultato[i] = PotenzaModulo(dati[i], Npri, N);
                    }
                }

                // Trasformiamo i valori numerici nei caratteri originali
                string messaggioOriginale = "";

                for (int i = 0; i < risultato.Length; i++)
                {
                    messaggioOriginale += (char)risultato[i];
                }

                // Mostriamo il messaggio decrittografato
                txtDecifrato.Text = messaggioOriginale;
            }
            catch
            {
                // Mostriamo un messaggio se il testo non è valido
                MessageBox.Show("Messaggio non valido!");
            }
        }
    }
}
