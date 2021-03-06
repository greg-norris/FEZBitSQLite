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
var font2 = FEZBitSQLite.Resource1.GetFont(FEZBitSQLite.Resource1.FontResources.GC_pixel);

InitDisplay();

screen.Clear();
screen.DrawImage(image, 0, 0);
screen.Flush();
Thread.Sleep(1000);

screen.DrawImage(image2, 0, 0);
screen.Flush();
Thread.Sleep(1000);

screen.Clear();
screen.Flush();

using (var db = new SQLiteDatabase())
{

    db.ExecuteNonQuery("CREATE TABLE Agent (Name TEXT, City TEXT, Sales DOUBLE);");


    db.ExecuteNonQuery("INSERT INTO Agent(Name, City, Sales) VALUES ('Bob', 'Dallas', 20375.69);");
    db.ExecuteNonQuery("INSERT INTO Agent(Name, City, Sales) VALUES('Karen', 'Atlanta', 24145.51);");
    db.ExecuteNonQuery("INSERT INTO Agent(Name, City, Sales) VALUES('Steve', 'Detroit', 16934.04);");
    db.ExecuteNonQuery("INSERT INTO Agent(Name, City, Sales) VALUES('Cindy', 'Austin', 12934.04);");
    db.ExecuteNonQuery("INSERT INTO Agent(Name, City, Sales) VALUES('Gus', 'Orlando', 25924.04);");
    db.ExecuteNonQuery("INSERT INTO Agent(Name, City, Sales) VALUES('Greg', 'Seattle', 15934.04);");
    db.ExecuteNonQuery("INSERT INTO Agent(Name, City, Sales) VALUES('Tim', 'Portland', 18934.04);");

    var result1 = db.ExecuteQuery("SELECT Name FROM Agent WHERE Sales > 10000;");
    var result2 = db.ExecuteQuery("SELECT City FROM Agent WHERE Sales > 10000;");
    var result3 = db.ExecuteQuery("SELECT Sales FROM Agent WHERE Sales > 10000;");


    screen.DrawString("Creating Table...", font, yellow, 10, 20);
    screen.Flush();
    Thread.Sleep(500);
    screen.DrawString("Intializing Data...", font, yellow, 10, 45);
    screen.Flush();
    Thread.Sleep(500);
    screen.DrawString("Quering Data...", font, yellow, 10, 70);
    screen.Flush();
    Thread.Sleep(500);

    screen.Clear();
    screen.Flush();

    //GC.Collect();
    //GC.WaitForPendingFinalizers();


    screen.DrawString("Name", font, yellow,0, 0);

    screen.DrawString("City", font, yellow, 60, 0);

    screen.DrawString("Sales", font, yellow, 115, 0);


    var str = "";
    var x = 30;
    var y = 5;
    foreach (ArrayList i in result1.Data)
    {
        str = "";

        foreach (object j in i)
            str += j.ToString() + " ";

        screen.DrawString(str, font2, white, y, x);
        screen.Flush();
        Thread.Sleep(50);
        x=x+15;

    }
    x = 30;
    y = y + 60;
    foreach (ArrayList i in result2.Data)
    {
        str = "";

        foreach (object j in i)
            str += j.ToString() + " ";

        screen.DrawString(str, font2, white, y, x);
        screen.Flush();
        Thread.Sleep(50);
        x = x + 15;

    }
    x = 30;
    y = y + 55;
    foreach (ArrayList i in result3.Data)
    {
        str = "";

        foreach (object j in i)
            str += j.ToString() + " ";

        screen.DrawString(str, font2, white, y, x);
        screen.Flush();
        Thread.Sleep(50);
        x = x + 15;

    }

    //foreach (ArrayList i in result2.Data)
    //{
    //    str = "";

    //    foreach (object j in i)
    //        str += j.ToString() + " ";

    //    screen.DrawString(str, font, white, 10, 70);


    //}
    //screen.Flush();
    //Thread.Sleep(2000);

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


