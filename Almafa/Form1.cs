using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


/*Hozz létre egy fát és egy embert, az embernek nem kötlező hogy feje legyen
Hozz létre egy kosarat
Az ember a kosár és a fa törzse között sétál oda-vissza
Amikor a fához ér, elkezdi azt a kezével ütni. Az ütés animálása kötelező
Miután az ember 10-szer megütötte a fát, egy alma hulljon le a lombból, pont az ember kezébe. A kezénél álljon meg
Amikor megvolt a 10. ütés, nem kell többet ütni, már csak a hulló almára vár az ember
Amint elkapta az almát, visszafordul, ez lehet egy hirtelen fordulás, majd elindul almával a kezében vissza a kosárhoz
Amikor elérte a kosarat, a kezében lévő alma folytatja lefelé hullását addig, amíg a kosár teljesen el nem nyeli
Amikor a kosár elnyelte az almát, a gyűjtött almák száma eggyel nő
  Ha a gyűjtött almák száma elérte a kosár teherbírását, több almát nem tárolunk el
Amint leesett az alma és elnyelte a kosár, az ember visszafordul és újrakezdi útját

A nagyobb kosár vásárlása gomb illetve a gyorsabb szedés gomb csak akkor működik, ha van elég almánk a fejlesztés megvásárlásához.
   nagyobb kosár vásárlása esetén:
     a kosár teherbírása 5 almával nő
     a nagyobb kosár ára 2 almával nő
   gyorsabb szedés vásárlása esetén:
    az alma leszedéséhez 1-el kevesebb ütés szükséges, de minimum 3
    a gyorsabb szedés ára 30 almával nő*/


namespace Almafa
{
    public partial class Form1 : Form
    {
        Timer walkTimer = new Timer();
        Timer hitTimer = new Timer();
        Timer gravity = new Timer();

        int hitCount = 0;
        int hitDirection = 0; // 1: balra; 0: nyugalmi állapot; -1: jobbra
        int hitFrames = 0;
        int hitMaxFrames = 3;
        int requiredHits = 10;
        int apples = 0;
        int storageCapacity = 20;
        int priceOfBiggerStorage = 10;
        int priceOfFasterHit = 30;

        bool holdingAnApple = false;
        bool facingLeft = true;

        public Form1()
        {
            InitializeComponent();
            Start();
        }
        void Start()
        {
            StartTimers();
            AddEvents();
        }
        void AddEvents()
        {
            BuyBiggerStorag.Click += BuyBiggerStorage;
            buyFasterHit.Click += BuyFasterHit;
        }
        void StartTimers()
        {
            walkTimer.Interval = 16;
            hitTimer.Interval = 16;
            gravity.Interval = 16;
            walkTimer.Start();
            walkTimer.Tick += walkEvent;
            hitTimer.Tick += hitEvent;
            gravity.Tick += GravityEvent;
        }
        void walkEvent(object s, EventArgs e)
        {
            if (kez.Left > torzs.Right && !holdingAnApple)
            {
                fej.Left -= 3;
                test.Left -= 3;
                kez.Left -= 3;
            }
            else if (!holdingAnApple)
            {
                walkTimer.Stop();
                hitDirection = 1;
                hitTimer.Start();
            }
            else if (holdingAnApple && test.Right < kosar.Left)
            {
                fej.Left += 3;
                test.Left += 3;
                kez.Left += 3;
                alma.Left += 3;
            }
            else if (holdingAnApple && test.Right >= kosar.Left)
            {
                walkTimer.Stop();
                gravity.Start();
            }            
        }
        void hitEvent(object s, EventArgs e)
        {
            if (hitDirection == 1)
            {
                kez.Left -= 6; // gyorsabb legyen, mint az ember mozgása (ami: 3 pixel)!
                hitFrames++;
                if (hitFrames == hitMaxFrames)
                {
                    hitDirection = -1;
                    hitFrames = 0;
                }
            }
            else if (hitDirection == -1)
            {
                kez.Left += 6; 
                hitFrames++;
                if (hitFrames == hitMaxFrames)
                {
                    hitFrames = 0;
                    hitCount++;
                    this.Text = hitCount.ToString();
                    if (hitCount == requiredHits)
                    {
                        hitDirection = 0;
                        hitTimer.Stop();
                        alma.Left = kez.Left;
                        alma.Top = lomb.Bottom - alma.Height;
                        alma.Show(); // alma.Visible = True;
                        gravity.Start();
                    }
                    else
                    {
                        hitDirection = 1;
                    }
                }
            }
        }
        void GravityEvent(object s, EventArgs e)
        {
            if (alma.Bottom < kez.Top && !holdingAnApple)
            {
                alma.Top += 3;
            }
            else if (!holdingAnApple)
            {
                holdingAnApple = true;
                gravity.Stop();
                kez.Left = test.Left + test.Width / 2;
                alma.Left = kez.Right - alma.Width;
                walkTimer.Start();
            }
            else if (holdingAnApple)
            {
                alma.Top += 3;
                if (alma.Top > kosar.Top)
                {
                    gravity.Stop();
                    holdingAnApple = false; // Itt már nem tartjuk az almát!
                    kez.Left = test.Left + test.Width / 2 - kez.Width;
                    hitCount = 0;
                    if (apples < storageCapacity)
                    {
                        apples++;
                    }
                    UpdateAppleLabel();

                    walkTimer.Start();
                }
            }
        }

        void UpdateAppleLabel()
        {
            appleCounter.Text = $"Gyűjtött almák száma: {apples}";
        }
        //Kosár teherbírása: 20 alma
        void UpdateStorageLabel()
        {
            storageLabel.Text = $"Kosár teherbírása: {storageCapacity} alma";
        }

        void BuyBiggerStorage(object sender, EventArgs e)
        {
            if (apples >= priceOfBiggerStorage)
            {
                apples -= priceOfBiggerStorage;
                storageCapacity += 5;
                priceOfBiggerStorage += 2;
                BuyBiggerStorag.Text = $"{priceOfBiggerStorage} alma";

                UpdateAppleLabel();
                UpdateStorageLabel();
            }
        }
        void BuyFasterHit(object sender, EventArgs e)
        {
            if (apples >= priceOfFasterHit && requiredHits > 3)
            {
                apples -= priceOfFasterHit;
                requiredHits--;
                priceOfFasterHit += 30;
                buyFasterHit.Text = $"{priceOfFasterHit} alma";

                UpdateAppleLabel();
            }
        }
    }
}

// Eltünik az alma a kosárban: Form1.cs[Design] / Kosár/ jobb gomb/ Bring to front
