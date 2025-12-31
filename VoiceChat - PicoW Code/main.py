import socket
import network
import time
from machine import Pin

# ---------------- CONFIG ----------------
SSID = ""
PASSWORD = ""
PORT = 5005

WIFI_TIMEOUT = 10
RETRY_DELAY = 5

FAST_BLINK = 0.2
SLOW_BLINK = 1.0
# ---------------------------------------

led = Pin("LED", Pin.OUT)

wlan = network.WLAN(network.STA_IF)
wlan.active(True)

sock = None
clients = set()

# ---------- LED HELPERS ----------
def led_fast():
    led.toggle()
    time.sleep(FAST_BLINK)

def led_slow():
    led.toggle()
    time.sleep(SLOW_BLINK)

def led_on():
    led.value(1)

def led_off():
    led.value(0)
# --------------------------------


def connect_wifi():
    print("Connecting to WiFi...")
    wlan.disconnect()
    wlan.connect(SSID, PASSWORD)

    start = time.time()
    led_off()

    while not wlan.isconnected():
        if time.time() - start > WIFI_TIMEOUT:
            print("WiFi timeout")
            return False
        led_fast()

    print("WiFi connected:", wlan.ifconfig())
    return True


def setup_socket():
    global sock
    if sock:
        sock.close()

    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.bind(("0.0.0.0", PORT))
    sock.setblocking(False)

    print("UDP socket ready on port", PORT)


# ---------------- MAIN LOOP ----------------
while True:

    # ---- Ensure Wi-Fi ----
    if not wlan.isconnected():
        print("WiFi disconnected")
        while not connect_wifi():
            print("Retrying WiFi in", RETRY_DELAY, "seconds")
            for _ in range(int(RETRY_DELAY / SLOW_BLINK)):
                led_slow()

        setup_socket()
        clients.clear()
        led_on()
        print("Voice relay running")

    # ---- UDP Relay ----
    try:
        data, addr = sock.recvfrom(2048)
        clients.add(addr)

        for c in clients:
            if c != addr:
                sock.sendto(data, c)

    except:
        pass

    time.sleep(0.001)