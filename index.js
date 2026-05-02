const main = document.getElementById("main");
const body = document.body;

const i18n = {
    cs: {
        home_title: "NetherOS",
        products_title: "Produkty - NetherOS",
        help_title: "Pomoc - NetherOS",

        calculator_title: "Kalkulačka - Produkty - NetherOS",
        clocks_title: "Hodiny - Produkty - NetherOS",
        explorer_title: "Průzkumík souborů - Produkty - NetherOS",
        music_title: "Hudba - Produkty - NetherOS",
        photos_title: "Fotky - Produkty - NetherOS",
        server_title: "Server - Produkty - NetherOS",
        system_monitor_title: "Sledování systému - Produkty - NetherOS",
        terminal_title: "Terminál - Produkty - NetherOS",
        ui_title: "UI - Produkty - NetherOS",
        videos_title: "Videos - Produkty - NetherOS",

        products: "Produkty",
        help: "Pomoc",
        go_back: "Zpět",

        calculator: "Kalkulačka",
        clocks: "Hodiny",
        explorer: "Průzkumník souborů",
        music: "Hudba",
        photos: "Fotky",
        server: "Server",
        system_monitor: "Sledování systému",
        terminal: "Terminál",
        ui: "UI",
        videos: "Videa",

        download_portable: "Stáhnout Přenosnou verzi",
        download_installer: "Stahnout Instalátor"
    },
    sk: {
        home_title: "NetherOS",
        products_title: "Produkty - NetherOS",
        help_title: "Pomoc - NetherOS",

        calculator_title: "Kalkulačka - Produkty - NetherOS",
        clocks_title: "Hodiny - Produkty - NetherOS",
        explorer_title: "Prieskumník súborov - Produkty - NetherOS",
        music_title: "Hudba - Produkty - NetherOS",
        photos_title: "Fotky - Produkty - NetherOS",
        server_title: "Server - Produkty - NetherOS",
        system_monitor_title: "Monitorovanie systému - Produkty - NetherOS",
        terminal_title: "Terminál - Produkty - NetherOS",
        ui_title: "UI - Produkty - NetherOS",
        videos_title: "Videa - Produkty - NetherOS",

        products: "Produkty",
        help: "Pomoc",
        go_back: "Späť",

        calculator: "Kalkulačka",
        clocks: "Hodiny",
        explorer: "Prieskumník súborov",
        music: "Hudba",
        photos: "Fotky",
        server: "Server",
        system_monitor: "Monitor systému",
        terminal: "Terminál",
        ui: "UI",
        videos: "Videá",

        download_portable: "Stiahnuť prenosnú verziu",
        download_installer: "Stiahnuť inštalátor"
    },
    pl: {
        home_title: "NetherOS",
        products_title: "Produkty - NetherOS",
        help_title: "Pomoc - NetherOS",

        calculator_title: "Kalkulator - Produkty - NetherOS",
        clocks_title: "Zegary - Produkty - NetherOS",
        explorer_title: "Eksplorator plików - Produkty - NetherOS",
        music_title: "Muzyka - Produkty - NetherOS",
        photos_title: "Zdjęcia - Produkty - NetherOS",
        server_title: "Serwer - Produkty - NetherOS",
        system_monitor_title: "Monitor systemu - Produkty - NetherOS",
        terminal_title: "Terminal - Produkty - NetherOS",
        ui_title: "UI - Produkty - NetherOS",
        videos_title: "Wideo - Produkty - NetherOS",

        products: "Produkty",
        help: "Pomoc",
        go_back: "Wstecz",

        calculator: "Kalkulator",
        clocks: "Zegary",
        explorer: "Eksplorator plików",
        music: "Muzyka",
        photos: "Zdjęcia",
        server: "Serwer",
        system_monitor: "Monitor systemu",
        terminal: "Terminal",
        ui: "UI",
        videos: "Wideo",

        download_portable: "Pobierz wersję przenośną",
        download_installer: "Pobierz instalator"
    },
    en: {
        home_title: "NetherOS",
        products_title: "Products - NetherOS",
        help_title: "Pomoc - NetherOS",

        calculator_title: "Kalkulačka - Products - NetherOS",
        clocks_title: "Hodiny - Products - NetherOS",
        explorer_title: "Průzkumík souborů - Products - NetherOS",
        music_title: "Hudba - Products - NetherOS",
        photos_title: "Fotky - Products - NetherOS",
        server_title: "Server - Products - NetherOS",
        system_monitor_title: "Sledování systému - Products - NetherOS",
        terminal_title: "Terminál - Products - NetherOS",
        ui_title: "UI - Products - NetherOS",
        videos_title: "Videos - Products - NetherOS",

        products: "Products",
        help: "Help",
        go_back: "Go back",

        calculator: "Calculator",
        clocks: "Clocks",
        explorer: "Explorer",
        music: "Music",
        photos: "Photos",
        server: "Server",
        system_monitor: "System Monitor",
        terminal: "Terminal",
        ui: "UI",
        videos: "Videos",

        download_portable: "Download portable version",
        download_installer: "Download installer"
    },
    de: {
        home_title: "NetherOS",
        products_title: "Produkte - NetherOS",
        help_title: "Hilfe - NetherOS",

        calculator_title: "Rechner - Produkte - NetherOS",
        clocks_title: "Uhren - Produkte - NetherOS",
        explorer_title: "Datei-Explorer - Produkte - NetherOS",
        music_title: "Musik - Produkte - NetherOS",
        photos_title: "Fotos - Produkte - NetherOS",
        server_title: "Server - Produkte - NetherOS",
        system_monitor_title: "Systemüberwachung - Produkte - NetherOS",
        terminal_title: "Terminal - Produkte - NetherOS",
        ui_title: "UI - Produkte - NetherOS",
        videos_title: "Videos - Produkte - NetherOS",

        products: "Produkte",
        help: "Hilfe",
        go_back: "Zurück",

        calculator: "Rechner",
        clocks: "Uhren",
        explorer: "Datei-Explorer",
        music: "Musik",
        photos: "Fotos",
        server: "Server",
        system_monitor: "Systemüberwachung",
        terminal: "Terminal",
        ui: "UI",
        videos: "Videos",

        download_portable: "Portable Version herunterladen",
        download_installer: "Installer herunterladen"
    },
    sv: {
        home_title: "NetherOS",
        products_title: "Produkter - NetherOS",
        help_title: "Hjälp - NetherOS",

        calculator_title: "Kalkylator - Produkter - NetherOS",
        clocks_title: "Klockor - Produkter - NetherOS",
        explorer_title: "Filutforskare - Produkter - NetherOS",
        music_title: "Musik - Produkter - NetherOS",
        photos_title: "Foton - Produkter - NetherOS",
        server_title: "Server - Produkter - NetherOS",
        system_monitor_title: "Systemövervakning - Produkter - NetherOS",
        terminal_title: "Terminal - Produkter - NetherOS",
        ui_title: "UI - Produkter - NetherOS",
        videos_title: "Videor - Produkter - NetherOS",

        products: "Produkter",
        help: "Hjälp",
        go_back: "Tillbaka",

        calculator: "Kalkylator",
        clocks: "Klockor",
        explorer: "Filutforskare",
        music: "Musik",
        photos: "Foton",
        server: "Server",
        system_monitor: "Systemövervakning",
        terminal: "Terminal",
        ui: "UI",
        videos: "Videor",

        download_portable: "Ladda ner portabel version",
        download_installer: "Ladda ner installerare"
    },
    es: {
        home_title: "NetherOS",
        products_title: "Productos - NetherOS",
        help_title: "Ayuda - NetherOS",

        calculator_title: "Calculadora - Productos - NetherOS",
        clocks_title: "Relojes - Productos - NetherOS",
        explorer_title: "Explorador de archivos - Productos - NetherOS",
        music_title: "Música - Productos - NetherOS",
        photos_title: "Fotos - NetherOS",
        server_title: "Servidor - Productos - NetherOS",
        system_monitor_title: "Monitor del sistema - Productos - NetherOS",
        terminal_title: "Terminal - Productos - NetherOS",
        ui_title: "UI - Productos - NetherOS",
        videos_title: "Videos - Productos - NetherOS",

        products: "Productos",
        help: "Ayuda",
        go_back: "Atrás",

        calculator: "Calculadora",
        clocks: "Relojes",
        explorer: "Explorador de archivos",
        music: "Música",
        photos: "Fotos",
        server: "Servidor",
        system_monitor: "Monitor del sistema",
        terminal: "Terminal",
        ui: "UI",
        videos: "Videos",

        download_portable: "Descargar versión portátil",
        download_installer: "Descargar instalador"
    },
    fr: {
        home_title: "NetherOS",
        products_title: "Produits - NetherOS",
        help_title: "Aide - NetherOS",

        calculator_title: "Calculatrice - Produits - NetherOS",
        clocks_title: "Horloges - Produits - NetherOS",
        explorer_title: "Explorateur de fichiers - Produits - NetherOS",
        music_title: "Musique - Produits - NetherOS",
        photos_title: "Photos - Produits - NetherOS",
        server_title: "Serveur - Produits - NetherOS",
        system_monitor_title: "Surveillance système - Produits - NetherOS",
        terminal_title: "Terminal - Produits - NetherOS",
        ui_title: "UI - Produits - NetherOS",
        videos_title: "Vidéos - Produits - NetherOS",

        products: "Produits",
        help: "Aide",
        go_back: "Retour",

        calculator: "Calculatrice",
        clocks: "Horloges",
        explorer: "Explorateur de fichiers",
        music: "Musique",
        photos: "Photos",
        server: "Serveur",
        system_monitor: "Surveillance système",
        terminal: "Terminal",
        ui: "UI",
        videos: "Vidéos",

        download_portable: "Télécharger version portable",
        download_installer: "Télécharger l'installateur"
    },
    it: {
        home_title: "NetherOS",
        products_title: "Prodotti - NetherOS",
        help_title: "Aiuto - NetherOS",

        calculator_title: "Calcolatrice - Prodotti - NetherOS",
        clocks_title: "Orologi - Prodotti - NetherOS",
        explorer_title: "Esplora file - Prodotti - NetherOS",
        music_title: "Musica - Prodotti - NetherOS",
        photos_title: "Foto - Prodotti - NetherOS",
        server_title: "Server - Prodotti - NetherOS",
        system_monitor_title: "Monitor di sistema - Prodotti - NetherOS",
        terminal_title: "Terminale - Prodotti - NetherOS",
        ui_title: "UI - Prodotti - NetherOS",
        videos_title: "Video - Prodotti - NetherOS",

        products: "Prodotti",
        help: "Aiuto",
        go_back: "Indietro",

        calculator: "Calcolatrice",
        clocks: "Orologi",
        explorer: "Esplora file",
        music: "Musica",
        photos: "Foto",
        server: "Server",
        system_monitor: "Monitor di sistema",
        terminal: "Terminale",
        ui: "UI",
        videos: "Video",

        download_portable: "Scarica versione portatile",
        download_installer: "Scarica installer"
    },
    nl: {
        home_title: "NetherOS",
        products_title: "Producten - NetherOS",
        help_title: "Hulp - NetherOS",

        calculator_title: "Rekenmachine - Producten - NetherOS",
        clocks_title: "Klokken - Producten - NetherOS",
        explorer_title: "Bestandsverkenner - Producten - NetherOS",
        music_title: "Muziek - Producten - NetherOS",
        photos_title: "Foto's - Producten - NetherOS",
        server_title: "Server - Producten - NetherOS",
        system_monitor_title: "Systeemmonitor - Producten - NetherOS",
        terminal_title: "Terminal - Producten - NetherOS",
        ui_title: "UI - Producten - NetherOS",
        videos_title: "Video's - Producten - NetherOS",

        products: "Producten",
        help: "Hulp",
        go_back: "Terug",

        calculator: "Rekenmachine",
        clocks: "Klokken",
        explorer: "Bestandsverkenner",
        music: "Muziek",
        photos: "Foto's",
        server: "Server",
        system_monitor: "Systeemmonitor",
        terminal: "Terminal",
        ui: "UI",
        videos: "Video's",

        download_portable: "Download draagbare versie",
        download_installer: "Download installer"
    },
    no: {
        home_title: "NetherOS",
        products_title: "Produkter - NetherOS",
        help_title: "Hjelp - NetherOS",

        calculator_title: "Kalkulator - Produkter - NetherOS",
        clocks_title: "Klokker - Produkter - NetherOS",
        explorer_title: "Filutforsker - Produkter - NetherOS",
        music_title: "Musikk - Produkter - NetherOS",
        photos_title: "Bilder - Produkter - NetherOS",
        server_title: "Server - Produkter - NetherOS",
        system_monitor_title: "Systemovervåkning - Produkter - NetherOS",
        terminal_title: "Terminal - Produkter - NetherOS",
        ui_title: "UI - Produkter - NetherOS",
        videos_title: "Videoer - Produkter - NetherOS",

        products: "Produkter",
        help: "Hjelp",
        go_back: "Tilbake",

        calculator: "Kalkulator",
        clocks: "Klokker",
        explorer: "Filutforsker",
        music: "Musikk",
        photos: "Bilder",
        server: "Server",
        system_monitor: "Systemovervåkning",
        terminal: "Terminal",
        ui: "UI",
        videos: "Videoer",

        download_portable: "Last ned bærbar versjon",
        download_installer: "Last ned installasjonsprogram"
    },
    da: {
        home_title: "NetherOS",
        products_title: "Produkter - NetherOS",
        help_title: "Hjælp - NetherOS",

        calculator_title: "Lommeregner - Produkter - NetherOS",
        clocks_title: "Ure - Produkter - NetherOS",
        explorer_title: "Filutforsker - Produkter - NetherOS",
        music_title: "Musik - Produkter - NetherOS",
        photos_title: "Fotos - Produkter - NetherOS",
        server_title: "Server - Produkter - NetherOS",
        system_monitor_title: "Systemovervågning - Produkter - NetherOS",
        terminal_title: "Terminal - Produkter - NetherOS",
        ui_title: "UI - Produkter - NetherOS",
        videos_title: "Videoer - Produkter - NetherOS",

        products: "Produkter",
        help: "Hjælp",
        go_back: "Tilbage",

        calculator: "Lommeregner",
        clocks: "Ure",
        explorer: "Filutforsker",
        music: "Musik",
        photos: "Fotos",
        server: "Server",
        system_monitor: "Systemovervågning",
        terminal: "Terminal",
        ui: "UI",
        videos: "Videoer",

        download_portable: "Download bærbar version",
        download_installer: "Download installationsprogram"
    },
ro: {
        home_title: "NetherOS",
        products_title: "Produse - NetherOS",
        help_title: "Ajutor - NetherOS",

        calculator_title: "Calculator - Produse - NetherOS",
        clocks_title: "Ceasuri - Produse - NetherOS",
        explorer_title: "Explorer de fișiere - Produse - NetherOS",
        music_title: "Muzică - Produse - NetherOS",
        photos_title: "Fotografii - Produse - NetherOS",
        server_title: "Server - Produse - NetherOS",
        system_monitor_title: "Monitorizare sistem - Produse - NetherOS",
        terminal_title: "Terminal - Produse - NetherOS",
        ui_title: "UI - Produse - NetherOS",
        videos_title: "Videoclipuri - Produse - NetherOS",

        products: "Produse",
        help: "Ajutor",
        go_back: "Înapoi",

        calculator: "Calculator",
        clocks: "Ceasuri",
        explorer: "Explorer fișiere",
        music: "Muzică",
        photos: "Fotografii",
        server: "Server",
        system_monitor: "Monitorizare sistem",
        terminal: "Terminal",
        ui: "UI",
        videos: "Videoclipuri",

        download_portable: "Descarcă versiunea portabilă",
        download_installer: "Descarcă instalatorul"
    },
    hu: {
        home_title: "NetherOS",
        products_title: "Termékek - NetherOS",
        help_title: "Súgó - NetherOS",

        calculator_title: "Számológép - Termékek - NetherOS",
        clocks_title: "Órák - Termékek - NetherOS",
        explorer_title: "Fájlkezelő - Termékek - NetherOS",
        music_title: "Zene - Termékek - NetherOS",
        photos_title: "Fotók - Termékek - NetherOS",
        server_title: "Szerver - Termékek - NetherOS",
        system_monitor_title: "Rendszerfigyelő - Termékek - NetherOS",
        terminal_title: "Terminál - Termékek - NetherOS",
        ui_title: "UI - Termékek - NetherOS",
        videos_title: "Videók - Termékek - NetherOS",

        products: "Termékek",
        help: "Súgó",
        go_back: "Vissza",

        calculator: "Számológép",
        clocks: "Órák",
        explorer: "Fájlkezelő",
        music: "Zene",
        photos: "Fotók",
        server: "Szerver",
        system_monitor: "Rendszerfigyelő",
        terminal: "Terminál",
        ui: "UI",
        videos: "Videók",

        download_portable: "Hordozható verzió letöltése",
        download_installer: "Telepítő letöltése"
    },
    tr: {
        home_title: "NetherOS",
        products_title: "Ürünler - NetherOS",
        help_title: "Yardım - NetherOS",

        calculator_title: "Hesap Makinesi - Ürünler - NetherOS",
        clocks_title: "Saatler - Ürünler - NetherOS",
        explorer_title: "Dosya Gezgini - Ürünler - NetherOS",
        music_title: "Müzik - Ürünler - NetherOS",
        photos_title: "Fotoğraflar - Ürünler - NetherOS",
        server_title: "Sunucu - Ürünler - NetherOS",
        system_monitor_title: "Sistem İzleyici - Ürünler - NetherOS",
        terminal_title: "Terminal - Ürünler - NetherOS",
        ui_title: "UI - Ürünler - NetherOS",
        videos_title: "Videolar - Ürünler - NetherOS",

        products: "Ürünler",
        help: "Yardım",
        go_back: "Geri",

        calculator: "Hesap Makinesi",
        clocks: "Saatler",
        explorer: "Dosya Gezgini",
        music: "Müzik",
        photos: "Fotoğraflar",
        server: "Sunucu",
        system_monitor: "Sistem İzleyici",
        terminal: "Terminal",
        ui: "UI",
        videos: "Videolar",

        download_portable: "Taşınabilir sürümü indir",
        download_installer: "Yükleyiciyi indir"
    }
};

const lang = navigator.language.slice(0, 2);
const t = i18n[lang] || i18n.en;



function showHome() {
    document.title = t.home_title;

    body.innerHTML = `
        <header>
            <button onclick="showHome()">
                <img src="img/logo.svg" alt="">
            </button>
        </header>

        <main>
            <div class="box vertical">
                <button class="sub-box button" onclick="showProducts()">${t.products}</button>
                <br>
                <button class="sub-box button" onclick="showHelp()">${t.help}</button>
            </div>
        </main>
    `;
}

function showProducts() {
    document.title = t.products_title;

    body.innerHTML = `
        <header>
            <button onclick="showHome()">
                <img src="img/logo.svg" alt="">
            </button>
        </header>

        <main>
            <div class="box vertical">

                <button class="sub-box button" onclick="showHome()">
                    <img src="img/go-back.svg" alt="">
                    ${t.go_back}
                </button>
                <br>

                <button class="sub-box button" onclick="showProductPage('Calculator', 'calculator')">
                    <img src="source-codes/calculator/img/logo.svg" alt="">
                    ${t.calculator}
                </button>
                <br>

                <button class="sub-box button" onclick="showProductPage('Clocks', 'clocks')">
                    <img src="source-codes/clocks/img/logo.svg" alt="">
                    ${t.clocks}
                </button>
                <br>

                <button class="sub-box button" onclick="showProductPage('Explorer', 'explorer')">
                    <img src="source-codes/explorer/img/logo.svg" alt="">
                    ${t.explorer}
                </button>
                <br>

                <button class="sub-box button" onclick="showProductPage('Music Player', 'music-player')">
                    <img src="source-codes/music/img/logo.svg" alt="">
                    ${t.music}
                </button>
                <br>

                <!--<button class="sub-box button" onclick="showProductPage('Photo Viewer', 'photo-viewer')">
                    <img src="source-codes/photo-viewer/img/logo.svg" alt="">
                    ${t.photos}
                </button>
                <br>-->

                <button class="sub-box button" onclick="showProductPage('Server', 'server')">
                    <img src="source-codes/server/img/logo.svg" alt="">
                    ${t.server}
                </button>
                <br>

                <!--<button class="sub-box button" onclick="showProductPage('System Monitor', 'system-monitor')">
                    <img src="source-codes/system-monitor/img/logo.svg" alt="">
                    ${t.system_monitor}
                </button>
                <br>-->

                <button class="sub-box button" onclick="showProductPage('Terminal', 'terminal')">
                    <img src="source-codes/terminal/img/logo.svg" alt="">
                    ${t.terminal}
                </button>
                <br>

                <button class="sub-box button" onclick="showProductPage('UI', 'ui')">
                    <img src="source-codes/ui/img/logo.svg" alt="">
                    ${t.ui}
                </button>

                <!--<br>
                <button class="sub-box button" onclick="showProductPage('Video Player', 'video-player')">
                    <img src="source-codes/video-player/img/logo.svg" alt="">
                    ${t.videos}
                </button>-->

            </div>
        </main>
    `;
}

function showProductPage(nameUpperFirstLetter, nameLowerFirstLetter) {
    document.title = `${nameUpperFirstLetter} - Products - NetherOS`;

    body.innerHTML = `
        <header>
            <button onclick="showHome()">
                <img src="img/logo.svg" alt="">
            </button>
        </header>

        <main>
            <div class="box horizontal">

                <button class="sub-box button" onclick="showProducts()">
                    <img src="img/go-back.svg" alt="">
                </button>
                <br>

                <div class="sub-box vertical">
                    <h2>Windows 64-bit (x64)</h2>
                    <a class="button" href="downloads/nether-os-${nameLowerFirstLetter}-win-x64-portable.zip">${t.download_portable}</a>
                    <!--<a class="button" href="downloads/nether-os-${nameLowerFirstLetter}-win-x64-install.zip">${t.download_installer}</a>-->
                </div>
                <br>

                <div class="sub-box vertical">
                    <h2>Windows 32-bit (x86)</h2>
                    <a class="button" href="downloads/nether-os-${nameLowerFirstLetter}-win-x86-portable.zip">${t.download_portable}</a>
                    <!--<a class="button" href="downloads/nether-os-${nameLowerFirstLetter}-win-x86-install.zip">${t.download_installer}</a>-->
                </div>
                <br>

                <div class="sub-box vertical">
                    <h2>Windows ARM64 (arm-64)</h2>
                    <a class="button" href="downloads/nether-os-${nameLowerFirstLetter}-win-arm64-portable.zip">${t.download_portable}</a>
                    <!--<a class="button" href="downloads/nether-os-${nameLowerFirstLetter}-win-arm64-install.zip">${t.download_installer}</a>-->
                </div>

            </div>
        </main>
    `;
}

function showHelp() {
    document.title = "Help - NetherOS"

    body.innerHTML = `
        <header>
            <button onclick="showHome()">
                <img src="img/logo.svg" alt="">
            </button>
        </header>
        <main>

        </main>
    `
    showHome();
}

showHome();