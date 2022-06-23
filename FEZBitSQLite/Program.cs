using System;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Drivers.Sitronix.ST7735;
using GHIElectronics.TinyCLR.Pins;
using System.Drawing;
using GHIElectronics.TinyCLR.Drivers.Worldsemi.WS2812;
using System.Threading;
using GHIElectronics.TinyCLR.Drivers.BasicGraphics;
using System.Collections;
using System.Diagnostics;
using GHIElectronics.TinyCLR.Data.SQLite;
using GHIElectronics.TinyCLR.Native;

ST7735Controller st7735 = null;
Graphics.OnFlushEvent += Graphics_OnFlushEvent;
SolidBrush yellow = new SolidBrush(Color.Yellow);
SolidBrush white = new SolidBrush(Color.White);

const int NUM_LED = 8;
var pin = GpioController.GetDefault().OpenPin(FEZBit.GpioPin.P0);
var leds = new WS2812Controller(pin, NUM_LED, WS2812Controller.DataFormat.rgb888);
var screen = Graphics.FromImage(new Bitmap(160, 128));
var image = FEZBitSQLite.Resource1.GetBitmap(FEZBitSQLite.Resource1.BitmapResources.logo);
var image2 = FEZBitSQLite.Resource1.GetBitmap(FEZBitSQLite.Resource1.BitmapResources.SQLiteLogo);
var font = FEZBitSQLite.Resource1.GetFont(FEZBitSQLite.Resource1.FontResources.Play);

InitDisplay();

screen.Clear();
screen.DrawImage(image, 0, 0);
screen.Flush();
Thread.Sleep(2000);

screen.DrawImage(image2, 0, 0);
screen.Flush();
Thread.Sleep(2000);

screen.Clear();
screen.Flush();

using (var db = new SQLiteDatabase())
{

    db.ExecuteNonQuery("CREATE TABLE Stock (Name TEXT, High DOUBLE, Low DOUBLE);");

    screen.DrawString("Creating Table...", font, yellow, 10, 30);
    screen.Flush();
    Thread.Sleep(1000);
    screen.DrawString("Intializing Data...", font, yellow, 10, 45);
    screen.Flush();
    Thread.Sleep(1000);
    screen.DrawString("Quering Data...", font, yellow, 10, 60);
    screen.Flush();
    Thread.Sleep(1000);

    screen.Clear();
    screen.Flush();

    db.ExecuteNonQuery("INSERT INTO Stock(Name, High, Low) VALUES ('Google', 3030.93, 2037.69);");
    db.ExecuteNonQuery("INSERT INTO Stock(Name, High, Low) VALUES('Microsoft', 349.67, 241.51);");
    db.ExecuteNonQuery("INSERT INTO Stock(Name, High, Low) VALUES('Apple', 182.94, 129.04);");


    var result1 = db.ExecuteQuery("SELECT Name FROM Stock WHERE High > 350;");

    var result2 = db.ExecuteQuery("SELECT Name FROM Stock WHERE High < 200;");

    //GC.Collect();
    //GC.WaitForPendingFinalizers();

    screen.DrawString("Highest Stock:", font, yellow, 10, 10);
    screen.DrawString("Lowest Stock:", font, yellow, 10, 55);

    var str = "";

    foreach (ArrayList i in result1.Data)
    {
        str = "";

        foreach (object j in i)
            str += j.ToString() + " ";

        screen.DrawString(str, font, white, 10, 30);

       
    }

    foreach (ArrayList i in result2.Data)
    {
        str = "";

        foreach (object j in i)
            str += j.ToString() + " ";

        screen.DrawString(str, font, white, 10, 70);

       
    }
    screen.Flush();
    Thread.Sleep(2000);

}


void InitDisplay()
{
    // Display Get Ready ////////////////////////////////////
    var spi = SpiController.FromName(FEZBit.SpiBus.Display);
    var gpio = GpioController.GetDefault();
    st7735 = new ST7735Controller(
    spi.GetDevice(ST7735Controller.GetConnectionSettings
    (SpiChipSelectType.Gpio,
    gpio.OpenPin(FEZBit.GpioPin.DisplayChipselect))), //CS pin.
    gpio.OpenPin(FEZBit.GpioPin.DisplayRs), //RS pin.
    gpio.OpenPin(FEZBit.GpioPin.DisplayReset) //RESET pin.
    );
    var backlight = gpio.OpenPin(FEZBit.GpioPin.Backlight);
    backlight.SetDriveMode(GpioPinDriveMode.Output);
    backlight.Write(GpioPinValue.High);
    st7735.SetDataAccessControl(true, true, false, false); //Rotate the screen.
    st7735.SetDrawWindow(0, 0, 160, 128);
    st7735.Enable();
}

void Graphics_OnFlushEvent(Graphics sender, byte[] data, int x, int y, int width, int
height, int originalWidth)
{
    st7735.DrawBuffer(data);
}


