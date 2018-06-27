var okno = document.getElementById('okno').style;
var okno_przew = document.getElementById('okno_przew').style;
var okno_blad = document.getElementById('okno_blad').style;
var okno_blad_tekst = document.getElementById('okno_blad_tekst');
var okno_iloscos = document.getElementById('okno_iloscos').style;
var zrodlo = document.getElementById('zrodlo');
var cel = document.getElementById('cel');
var przewoznicy = document.getElementById('przewoznicy');
var os_dor = document.getElementById('os_dor');
var os_mlo = document.getElementById('os_mlo');
var os_dzi = document.getElementById('os_dzi');
var os_nie = document.getElementById('os_nie');
var zrodlo = document.getElementById('zrodlo');
var cel = document.getElementById('cel');
var data = document.getElementById('data');
var bezposrednie = document.getElementById('bezposrednie');
var podp_zrodlo = document.getElementById('podp_zrodlo');
var podp_cel = document.getElementById('podp_cel');
var zasoby = document.getElementById('zasoby').style;
var zas_proc = document.getElementById('zas_proc');
var zas_dysk = document.getElementById('zas_dysk');
var zas_ram = document.getElementById('zas_ram');
var zas_ile = document.getElementById('zas_ile');
var stickarena = document.getElementById('stickarena').style;
var funkcja_zamknijokno = null;
var zrodlo_tekst = '';
var cel_tekst = '';
var pokaz_podp_zr = false;
var pokaz_podp_cel = false;
var zrodlo_kod = '';
var cel_kod = '';
var loty = null;

const OSOBY_MAX = 9;

var ilosc_osob = {
	Dorosli:1,
	Mlodziez:0,
	Dzieci:0,
	Niemowleta:0
}

var animacja = {
	samolot: document.getElementById('wysz_sam'),
	brzoza: document.getElementById('wysz_brzoza'),
	wysz_div: document.getElementById('wysz_div'),
	wysz_pasek: document.getElementById('wysz_pasek').style,
	wysz_tytul: document.getElementById('wysz_tytul').style,
	sam_pozx: -70,
	sam_pozy: 0,
	stan: 0,
	obrot: 0,
	brzoza_pozy: -40,
	interwal: -1,
	obiekt: null,

	przesun_samolot: function(){
		var o = animacja.obiekt;
		
		if(o.stan == 0){
			var max = wysz_div.offsetWidth - o.samolot.offsetWidth - o.brzoza.offsetWidth;
			if(o.sam_pozx < max){
				o.sam_pozx += 4;
			}else{
				o.stan = 1;
			}
			o.samolot.style.left = o.sam_pozx + 'px';
			o.samolot.style.top = '0px';
			o.samolot.style.transform = 'rotate(0)';
			
			if(max - o.sam_pozx < 160){
				o.brzoza_pozy = ((max - o.sam_pozx) - 64)/2;
				if(o.brzoza_pozy >= 0) o.brzoza.style.top = o.brzoza_pozy + 'px';
			}else{
				o.brzoza_pozy = 0;
				o.brzoza.style.top = '-40px';
			}
			
		}else{
			o.sam_pozy += 2;
			o.obrot -= 5;
			o.samolot.style.top = o.sam_pozy + 'px';
			o.samolot.style.transform = 'rotate(' + o.obrot + 'deg)';
			if(o.sam_pozy > 30){
				o.sam_pozy = 0;
				o.sam_pozx = -70;
				o.stan = 0;
				o.obrot = 0;
			}
		}
	},

	uruchom_animacje: function(){
		if(this.interwal != -1) return;
		this.obiekt = this;
		this.interwal = setInterval(this.przesun_samolot, 20);
		this.sam_pozx = -70;
		this.sam_pozy = 0;
		this.stan = 0;
		this.obrot = 0;
		this.brzoza_pozy = -40;
		this.wysz_pasek.display = 'block';
		this.wysz_tytul.display = 'block';
	},

	zatrzymaj_animacje: function(){
		if(this.interwal == -1) return;
		clearInterval(this.interwal);
		this.interwal = -1;
		this.wysz_pasek.display = 'none';
		this.wysz_tytul.display = 'none';
	}
}

function zapytanie(adres, funkcja, metoda, dane) {
	var xhr = new XMLHttpRequest();
	xhr.onreadystatechange = (function (x) {
		return function () {
			if (x.readyState == 4) { funkcja(x); }
		}
	})(xhr);
	xhr.open(metoda, adres, true);
	xhr.setRequestHeader('Accept', 'application/json');
	if(dane != '') xhr.setRequestHeader('Content-Type', 'application/json');
	xhr.send(dane);
}

function dodaj_przewoznikow(e){
	if(e.status != 200){
		pokaz_blad('Usługa chwilowo niedostępna: błąd podczas pobierania listy przewoźników.');
		return;
	}
	
	var prz = eval('(' + e.responseText + ')');
	if(prz == null) return;
	
	for(var i = 0; i < prz.length; i++){
		var id = 'przew' + (i+1);
		
		var el = document.createElement('input');
		el.type = 'checkbox';
		el.checked = 'checked';
		el.value = prz[i];
		el.id = id;
		przewoznicy.appendChild(el);
		
		el = document.createElement('label');
		el.htmlFor = id;
		el.textContent = prz[i];
		przewoznicy.appendChild(el);
		
		el = document.createElement('br');
		przewoznicy.appendChild(el);
	}
}

function pokaz_przewoznikow(){
	okno.display = 'block';
	okno_przew.display = 'block';
	okno_przew.top = (window.pageYOffset + 15) + 'px';
}

function pokaz_blad(tekst){
	okno_blad_tekst.textContent = tekst;
	okno.display = 'block';
	okno_blad.display = 'block';
	okno_blad.top = (window.pageYOffset + 15) + 'px';
}

function pokaz_osoby(){
	okno.display = 'block';
	okno_iloscos.display = 'block';
	okno_iloscos.top = (window.pageYOffset + 15) + 'px';
}

function ukryj_okna(){
	okno.display = 'none';
	okno_przew.display = 'none';
	okno_blad.display = 'none';
	okno_iloscos.display = 'none';
}

function sprawdz_osoby(){
	var ilosc = ilosc_osob.Dorosli + ilosc_osob.Mlodziez + ilosc_osob.Dzieci + ilosc_osob.Niemowleta;
	if(ilosc == 0){
		pokaz_blad('Nie wybrano żadnej osoby.');
		return false;
	}
	if(ilosc > OSOBY_MAX){
		pokaz_blad('Zbyt dużo osób podróżuje na jednym bilecie. Największa liczba osób to ' + OSOBY_MAX + '.');
		return false;
	}
	if(ilosc_osob.Dorosli == 0){
		pokaz_blad('Życie to nie Kevin sam w Nowym Jorku - ktoś dorosły musi z wami lecieć.');
		return false;
	}
	if((ilosc - ilosc_osob.Dorosli) > ilosc_osob.Dorosli){
		pokaz_blad('Liczba osób niepełnoletnich nie może być większa niż liczba dorosłych.');
		return false;
	}
	return true;
}

function zamknij_osoby(){
	ukryj_okna();
	if(!sprawdz_osoby()){
		funkcja_zamknijokno = pokaz_osoby;
	}
}

function zamknij_blad(){
	ukryj_okna();
	if(funkcja_zamknijokno != null) funkcja_zamknijokno();
	funkcja_zamknijokno = null;
}

function zamien_miasta(){
	var miasto = zrodlo.value;
	zrodlo.value = cel.value;
	cel.value = miasto;
}

function pobierzM(minuty){
	var m = minuty + '';
	if(minuty<10) m = '0' + m;
	return m;
}

function utwWezel(nazwa, atrybuty, tekst){
	var w = document.createElement(nazwa);
	for(var i in atrybuty){
		var a = document.createAttribute(i);
		a.value = atrybuty[i];
		w.attributes.setNamedItem(a);
	}
	if(tekst != '') w.appendChild(document.createTextNode(tekst));
	return w;
}

function dodaj_wynik(el, i, j){
	var w = utwWezel('div', {class:'tr', id:'pol'+i}, '');
	
	//Zrodlo
	var div = utwWezel('div', {class:'td'}, '');
	var span = utwWezel('span', {class:'lotnisko'}, '');
	span.appendChild(utwWezel('span', {class:'opism'}, 'Z: '));
	span.appendChild(document.createTextNode(el.Zrodlo));
	div.appendChild(span);
	div.appendChild(document.createElement('br'));
	div.appendChild(utwWezel('span', {class:'data'}, el.DataWylotu.toLocaleDateString() + ' ' + el.DataWylotu.getHours() + ':' + pobierzM(el.DataWylotu.getMinutes())));
	w.appendChild(div);
	
	//Czas
	div = utwWezel('div', {class:'td'}, '');
	div.appendChild(utwWezel('span', {class:'opism'}, 'Czas: '));
	div.appendChild(document.createTextNode((el.Czas.getHours() + (el.Czas.getDate()-1)*24) + 'h '+ pobierzM(el.Czas.getMinutes()) + 'min'));
	w.appendChild(div);
	
	//Cel
	div = utwWezel('div', {class:'td'}, '');
	span = utwWezel('span', {class:'lotnisko'}, '');
	span.appendChild(utwWezel('span', {class:'opism'}, 'Do: '));
	span.appendChild(document.createTextNode(el.Cel));
	div.appendChild(span);
	div.appendChild(document.createElement('br'));
	div.appendChild(utwWezel('span', {class:'data'}, el.DataPrzylotu.getHours() + ':' + pobierzM(el.DataPrzylotu.getMinutes())));
	w.appendChild(div);
	
	//Przewoznik
	div = utwWezel('div', {class:'td'}, '');
	div.appendChild(utwWezel('span', {class:'opism'}, 'Przewoźnik: '));
	div.appendChild(document.createTextNode(el.Przewoznik));
	w.appendChild(div);
	
	//Cena
	div = utwWezel('div', {class:'td'}, '');
	div.appendChild(document.createTextNode('od ' + el.Cena + ' zł'));
	w.appendChild(div);
	
	//Link
	div = utwWezel('div', {class:'td'}, '');
	div.appendChild(utwWezel('a', {href:el.Link, class:'jasny'}, 'Kup bilet'));
	w.appendChild(div);

	polaczenia.appendChild(w);
}

function przetworz_element(el){
	el.DataWylotu = new Date(el.DataWylotu);
	el.DataPrzylotu = new Date(el.DataPrzylotu);
	var tab = el.Czas.split(':');
	var tab2 = tab[0].split('.');
	
	var godz;
	if(tab2.length == 1) godz = tab[0]*1; else godz = tab2[0]*24 + tab2[1]*1;
	
	var d = new Date(0);
	d.setMinutes(tab[1]*1);
	d.setHours(godz);
	el.Czas = d;
	return el;
}

function przetworz_polaczenia(e){
	animacja.zatrzymaj_animacje();
	
	if(e.status != 200){
		pokaz_blad('Wystąpił błąd podczas wyszukiwania połączeń.');
		return;
	}
	
	var pol = eval('(' + e.responseText + ')');
	if(pol == null) return;
	
	var zas = pol.Zasoby;
	pol = pol.Polaczenia;
	zasoby.display = 'block';
	zas_proc.textContent = Math.ceil(zas.ProcProcesora) + '%';
	zas_dysk.textContent = Math.ceil(zas.ProcDysku) + '%';
	zas_ram.textContent = Math.ceil(zas.ProcRAM) + '%';
	zas_ile.textContent = zas.ZajeteRAM;
	
	if(pol.length == 0) pokaz_blad('Nie znaleziono żadnych połączeń.');
	
	for(var i=0; i<pol.length; i++){
		pol[i] = przetworz_element(pol[i]);
		
		if(pol[i].Przesiadki != null){
			for(var j=0; j<pol[i].Przesiadki.length; j++){
				pol[i].Przesiadki[j] = przetworz_element(pol[i].Przesiadki[j]);
			}
		}
	}
	
	loty = pol;
	wyswietl_polaczenia(pol);
	loty = pol;	
}

function czysc_polaczenia(){
	var polaczenia = document.getElementById('polaczenia');
	for(var i=polaczenia.children.length-1; i > 0; i--){
		if(polaczenia.children[i].id != "pol_naglowek"){
			polaczenia.removeChild(polaczenia.children[i]);
		}
	}
}

function wyswietl_polaczenia(pol){
	czysc_polaczenia();

	for(var i=0; i<pol.length; i++){
		dodaj_wynik(pol[i], i+1, 0);
		
		if(pol[i].Przesiadki != null){
			for(var j=0; j<pol[i].Przesiadki.length; j++){
				dodaj_wynik(pol[i].Przesiadki[j], i+1, j+1);
			}
		}
	}
}

function wyslij_wyszukiwanie(){
	if(zrodlo.value == cel.value){
		pokaz_blad('Źródło i cel są takie same. Po co więc lecieć samolotem?');
		return;
	}
	
	if(zrodlo_kod == ''){
		pokaz_blad('Aby wybrać miejsce wylotu, wpisz fragment nazwy lotniska, następnie wybierz element z listy.');
		return;
	}
	
	if(cel_kod == ''){
		pokaz_blad('Aby wybrać miejsce przylotu, wpisz fragment nazwy lotniska, następnie wybierz element z listy.');
		return;
	}
	
	if(cel.value == 'Sosnowiec'){
		pokaz_blad('W Sosnowcu nie ma lotniska.');
		return;
	}
	
	var dzis = new Date();
	var d = new Date(data.value);
	
	if(isNaN(d.getYear())){
		pokaz_blad('Data wylotu jest niepoprawna.');
		return;
	}
	
	if(d < dzis){
		pokaz_blad('Funkcja podróży w czasie nie jest jeszcze obsługiwana. Wybierz datę z przyszłości.');
		return;
	}
	
	var p_tak = [];
	var p_nie = [];
	var i = 0;
	var el;
	
	while(true){
		el = document.getElementById('przew' + i);
		if(el == null) break;
		if(el.checked){
			p_tak[p_tak.length] = el.value;
		}else{
			p_nie[p_nie.length] = el.value;
		}
		i++;
	}
	
	if(p_tak.length == 0){
		pokaz_blad('Nie wybrano żadnego przewoźnika.');
		return;
	}
	
	if(!sprawdz_osoby()){
		funkcja_zamknijokno = null;
		return;
	}
	
	animacja.uruchom_animacje();
	if(stickarena.display != 'block') stickarena.display = 'block';
	
	var dane = {
		Zrodlo: zrodlo_kod,
		Cel: cel_kod,
		Data: d,
		PrzewTak: p_tak,
		PrzewNie: p_nie,
		Osoby: ilosc_osob,
		Bezposrednie: bezposrednie.checked
	}
	
	zapytanie('../Polaczenia', przetworz_polaczenia, 'POST', JSON.stringify(dane));
}

function resetuj_strone(){
	loty = null;
	animacja.zatrzymaj_animacje();
	zasoby.display = 'none';
	czysc_polaczenia();
}

function sort_cena_ros(a,b){
	return a.Cena - b.Cena;
}

function sort_cena_mal(a,b){
	return b.Cena - a.Cena;
}

function sort_czas_ros(a,b){
	if(a.Czas > b.Czas) return 1;
	if(a.Czas == b.Czas) return 0;
	if(a.Czas < b. Czas) return -1;
}

function sort_czas_mal(a,b){
	if(a.Czas > b.Czas) return -1;
	if(a.Czas == b.Czas) return 0;
	if(a.Czas < b. Czas) return 1;
}

function pobierz_kod(nazwa){
	var n1 = nazwa.lastIndexOf('(');
	var n2 = nazwa.lastIndexOf(')');
	if(n1 == -1 || n2 == -1) return '';
	return nazwa.slice(n1+1, n2);
}

function klik(e){
	var tag = e.target.tagName.toLowerCase()
	var id = e.target.id;
	
	//Rozwin/zwin przesiadki
	if(tag == 'img' && id.match(/^roz/) != null){
		var rozwin = (e.target.src.match(/dol\.png$/) != null);
		var styl1 = (rozwin ? 'wiersz_przes' : 'wiersz_przes_u');
		var styl2 = (rozwin ? 'wiersz_przes_u' : 'wiersz_przes');
		e.target.src = (rozwin ? 'Pliki/gora.png' : 'Pliki/dol.png');
		var nr = id.slice(3, id.length);
		var ident = 'pol' + nr + '_';
		var i = 1;
		
		while(true){
			var el = document.getElementById(ident + i);
			if(el == null) break;
			el.className = el.className.replace(styl1, styl2);
			i++;
		}
	}
	
	//Ilosc osob
	if(tag == 'img' && id.match(/^os_/) != null){
		var typ = id.slice(3, 6);
		var zm = (id.slice(7, 8) == 'm' ? -1 : 1);
		
		switch(typ){
			case 'dor':
				ilosc_osob.Dorosli += zm;
				if(ilosc_osob.Dorosli < 0 || ilosc_osob.Dorosli > OSOBY_MAX) ilosc_osob.Dorosli -= zm;
				os_dor.textContent = ilosc_osob.Dorosli;
				break;
				
			case 'mlo':
				ilosc_osob.Mlodziez += zm;
				if(ilosc_osob.Mlodziez < 0 || ilosc_osob.Mlodziez > OSOBY_MAX) ilosc_osob.Mlodziez -= zm;
				os_mlo.textContent = ilosc_osob.Mlodziez;
				break;
				
			case 'dzi':
				ilosc_osob.Dzieci += zm;
				if(ilosc_osob.Dzieci < 0 || ilosc_osob.Dzieci > OSOBY_MAX) ilosc_osob.Dzieci -= zm;
				os_dzi.textContent = ilosc_osob.Dzieci;
				break;
				
			case 'nie':
				ilosc_osob.Niemowleta += zm;
				if(ilosc_osob.Niemowleta < 0 || ilosc_osob.Niemowleta > OSOBY_MAX) ilosc_osob.Niemowleta -= zm;
				os_nie.textContent = ilosc_osob.Niemowleta;
				break;
		}
	}
	
	//Sortowanie
	if(id.match(/^sort/) != null && loty != null){
		var f = null;
		
		if(id.match(/dol/) != null){	//malejaco
			if(id.match(/cena/)){ f = sort_cena_mal; }
			if(id.match(/czas/)){ f = sort_czas_mal; }
		}
		
		if(id.match(/gora/) != null){	//rosnaco
			if(id.match(/cena/)){ f = sort_cena_ros; }
			if(id.match(/czas/)){ f = sort_czas_ros; }
		}
		
		if(f != null){
			loty.sort(f);
			wyswietl_polaczenia(loty);
		}
	}
	
	//Podpowiedzi
	if(e.target.className == 'podp_el'){
		var t = e.target.textContent;
		if(pokaz_podp_zr) {zrodlo.value = t; zrodlo_kod = pobierz_kod(t);}
		if(pokaz_podp_cel) {cel.value = t; cel_kod = pobierz_kod(t);}
	}
}

function klawisz(e){
	if(e.keyCode == 27) ukryj_okna();	//ESC
}

function pokaz_podp(e, zr){
	if(e.status != 200) return;
	
	var podp;
	if(zr){
		if(!pokaz_podp_zr) return;
		podp = podp_zrodlo;
	}else{
		if(!pokaz_podp_cel) return;
		podp = podp_cel;
	}
	
	podp.innerHTML = '';
	podp.style.display = 'block';
	var lotn = eval('(' + e.responseText + ')');
	if(lotn ==  null) return;
	
	for(var i=0; i<lotn.length; i++){
		var el = document.createElement('div');
		el.textContent = lotn[i];
		el.className = 'podp_el';
		podp.appendChild(el);
	}
}

function pobierz_podp(tekst, zr){
	zapytanie('../Lotniska?nazwa=' + tekst, function(e){ pokaz_podp(e, zr); }, 'GET', '');
}

function podp_klawisz(zr){
	if(zr){
		
		if(zrodlo.value != zrodlo_tekst){
			zrodlo_tekst = zrodlo.value;
			if(zrodlo_tekst.length > 4){
				pobierz_podp(zrodlo_tekst, zr);
			}else{
				podp_zrodlo.style.display = 'none';
			}
		}
	
	}else{
		
		if(cel.value != cel_tekst){
			cel_tekst = cel.value;
			if(cel_tekst.length > 4){
				pobierz_podp(cel_tekst, zr);
			}else{
				podp_cel.style.display = 'none';
			}
		}
		
	}
}

function opusc_zrodlo(){
	setTimeout(function(){
		podp_zrodlo.style.display='none';
		pokaz_podp_zr = false;
	}, 100);
}

function opusc_cel(){
	setTimeout(function(){
		podp_cel.style.display='none';
		pokaz_podp_cel = false;
	}, 100);
}

zapytanie('../Przewoznicy', dodaj_przewoznikow, 'GET', '');
zrodlo_kod = pobierz_kod(zrodlo.value);
cel_kod = pobierz_kod(cel.value);

document.getElementById('przew_div').addEventListener('click', pokaz_przewoznikow);
document.getElementById('ilosc_os').addEventListener('click', pokaz_osoby);
document.getElementById('wybierz_przew').addEventListener('click', ukryj_okna);
document.getElementById('zamien').addEventListener('click', zamien_miasta);
document.getElementById('tlo').addEventListener('click', ukryj_okna);
document.getElementById('zamknij_blad').addEventListener('click', zamknij_blad);
document.getElementById('iloscos_wybierz').addEventListener('click', zamknij_osoby);
document.getElementById('szukaj').addEventListener('click', wyslij_wyszukiwanie);
document.getElementById('tytul').addEventListener('click', resetuj_strone);
zrodlo.addEventListener('blur', opusc_zrodlo);
zrodlo.addEventListener('keyup', function(){podp_klawisz(true); pokaz_podp_zr = true; zrodlo_kod = '';});
cel.addEventListener('blur', opusc_cel);
cel.addEventListener('keyup', function(){podp_klawisz(false); pokaz_podp_cel = true; cel_kod = '';});
document.addEventListener('click', klik);
document.addEventListener('keydown', klawisz);