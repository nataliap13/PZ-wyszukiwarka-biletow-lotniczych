from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.by import By


import string
import random
import time
import re
import linecache
import actions
from selenium.webdriver.common.proxy import Proxy, ProxyType

from selenium import webdriver

#citystart="Poznań"
#cityend="Monachium"
#month="Czerwiec"
#year="2018"
#day="15"

#adults=int(3)
#jungers=int(3)
#children=int(1)
#babies=int(2)

www="http://www.esky.pl"
#driver=webdriver.Chrome()
#driver.implicitly_wait(30)
#driver.get(www)


chrome_options = webdriver.ChromeOptions()
chrome_options.add_argument("--disable-notifications")
driver = webdriver.Chrome(chrome_options=chrome_options)


driver.get(www)
driver.maximize_window()
#driver.execute_script("document.body.style.zoom='70%'")
#dol=driver.find_element_by_xpath("//*[contains(@class,'action-query')]")
while(True):
    dol=driver.find_element_by_tag_name("body")
    dol.send_keys(Keys.END)
    citystart =input()
    cityend = input()
    month = input()
    year = input()
    day = input()

    adults =int(input())
    jungers =int(input())
    children =int(input())
    babies =int(input())


    #wybor w jedna strone
    OneWayTicket=driver.find_element_by_xpath("//*[@id='TripTypeOneway']")
    OneWayTicket.click()
    time.sleep(1)

    try:
        #wybor miejsca wylotu
        cit_start=driver.find_element_by_xpath("//*[@id='departureOneway']")
        cit_start.send_keys(citystart)
        #cit_start.send_keys(Keys.ENTER)

        time.sleep(1)

        #wybor wszystkich lotnisk z miejsca wylotu
        Alllotniska=driver.find_element_by_xpath("(//*[@data-city-with-country])[1]")
        Alllotniska.click()

        time.sleep(1)

        #wybor miejsca przylotu
        cit_end=driver.find_element_by_xpath("//*[@id='arrivalOneway']")
        cit_end.send_keys(cityend)
        cit_end.send_keys(Keys.ENTER)

        time.sleep(0.5)


        #JESLI ZNajdzie ze dla miasta docelowego jest kilka lotnisk
        warunek=driver.find_elements_by_xpath("(//*[contains(text(), 'wszystkie lotniska')])[2]")
        if(len(warunek)!=0):
            warunek[0].click()
        else:
            driver.find_element_by_xpath("//*[contains(@data-suggestion, '"+cityend+"')]").click()
    

        ########################
        #wybor daty

        while True:
            #print("while") 
            m=driver.find_elements_by_xpath("//*[@class='ui-datepicker-month' and text()='"+month+"']") #szukam miesiaca zadanego
            y=driver.find_elements_by_xpath("//*[@class='ui-datepicker-year' and text()='"+year+"']")   #szukam roku zadanego
            time.sleep(0.3)
            if(len(m)==0 or len(y)==0): #jesli nie ma ani miesiaca  lub roku to wtedy klika dalej
                #print("if")
                driver.find_element_by_xpath("//*[contains(text(), 'Następny')]").click()  # jesli nie ma to kilkam dalej
            else:
                #print("break") #jesli znajdzie to robie break
                break

        d=driver.find_element_by_xpath("//*[contains(@class,'ui-state')  and (text()= '"+day+"')]").click()

        # liczymy pasazerow
        #driver.find_elements_by_xpath("//*[@class='minus']")
        driver.find_elements_by_xpath("//*[@class='plus']")
        for x in range(0,adults-1):
            driver.find_element_by_xpath("(//*[@class='plus'])[1]").click()

        for x in range(0,jungers):
            driver.find_element_by_xpath("(//*[@class='plus'])[2]").click()

        for x in range(0,children):
            driver.find_element_by_xpath("(//*[@class='plus'])[3]").click()

        for x in range(0,babies):
            driver.find_element_by_xpath("(//*[@class='plus'])[4]").click()

        time.sleep(1)
        driver.find_element_by_xpath("(//*[contains(text(), 'Gotowe')])[2]").click()


        #END PROCESS  MAIN SUBMIT
        time.sleep(0.5)
        driver.find_element_by_xpath("//*[@class='btn transaction qsf-search']").click()


        WebDriverWait(driver,8 ).until(
        EC.element_to_be_clickable((By.XPATH, "(//*[text()='Wybierz'])[1]")))
        #driver.find_element_by_xpath("(//*[text()='Wybierz'])[1]")


        #########################################################################################
        # czekamy az strona sie przeladuje na nowa
        WebDriverWait(driver,8 ).until(
        EC.presence_of_element_located((By.XPATH, "//*[@class='current-price']//*[@class='amount']")))

        dol=driver.find_element_by_xpath("//*[contains(@class,'action-query')]")
        dol.send_keys(Keys.END)
        dol.send_keys(Keys.END)
        dol.send_keys(Keys.END)
        time.sleep(0.5)
        dol.send_keys(Keys.HOME)
        dol.send_keys(Keys.HOME)
        dol.send_keys(Keys.HOME)
        time.sleep(3)

        # ten xpath zliczy mi ile w kazym jest tych  poziomow i ile buttonow w kazdym
        '''
        T=[None] * 11
        for x in range(1,11):
            el=driver.find_elements_by_xpath("(//*[contains(@class,'leg-group-container')])["+str(x)+"] //input[@type='radio' and contains(@name,'Fligh')]")
            T[x]=len(el)
        '''

        Nazwa_Przewoznika=driver.find_elements_by_xpath("(//*[contains(@class,'square-logo')])") #.get_attribute("alt")   #10

        Ceny_biletow=driver.find_elements_by_xpath("(//*[@class='current-price']//*[@class='amount'])") #.get_attribute('textContent')  #10

        Lotnisko_Odlotu=driver.find_elements_by_xpath("//div[contains(@class,'departure-airport')]") #.get_attribute('textContent') #10 ale co dwa

        Lotnisko_przylotu=driver.find_elements_by_xpath("//div[contains(@class,'arrival-airport')]") #.get_attribute('textContent')  #10 ale co dwa 

        Godzina_odlotu=driver.find_elements_by_xpath("//*[@class='leg selected with-facilities']//*[@class='hour departure']") #.get_attribute('textContent')  #10

        Godzina_przylotu=driver.find_elements_by_xpath("//*[@class='leg selected with-facilities']//*[@class='hour arrival']")  #.get_attribute('textContent')    #10

        Czas_lotu=driver.find_elements_by_xpath("//*[@class='leg selected with-facilities']//*[@class='time']") #.get_attribute('textContent')    #10

        datastring=driver.find_elements_by_xpath("//*[@class='leg selected with-facilities']//*[contains(@data-tooltip,'arrival-day')]") #.get_attribute('textContent')

        hrefy=driver.find_elements_by_xpath("//*[@class='aside-wrapper']//*[contains(@href,'https://secure')]")

        print(len(Lotnisko_Odlotu))
        for x in range(len(Lotnisko_Odlotu)):
            print(Nazwa_Przewoznika[x].get_attribute("alt"))
            print(Ceny_biletow[x].get_attribute("textContent"))
            print(Lotnisko_Odlotu[x].get_attribute("textContent"))
            print(Lotnisko_przylotu[x].get_attribute("textContent"))
            print(Godzina_odlotu[x].get_attribute("textContent"))
            print(Godzina_przylotu[x].get_attribute("textContent"))
            print(Czas_lotu[x].get_attribute("textContent"))
            #print(datastring[x].get_attribute("textContent"))
            print(hrefy[x].get_attribute("href"))
            print("----------------------------------------------------------------------")


        driver.delete_all_cookies()
        driver.execute_script('window.localStorage.clear()')
        driver.get(www)
    except Exception as e:
        print("000")
        driver.delete_all_cookies()
        driver.execute_script('window.localStorage.clear()')
        driver.get(www)


