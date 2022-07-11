using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace game1
{
    class enemy_bullet
    {
        public int x, y, velx;
        public float w = 0.5f, h = 0.25f;
        public map m;
        public bool active = true;
        public void anim()
        {
            int i, j, j2;
            i = y / m.tile_height;

            if (velx > 0)
            {
                j = (x + (int)(w * m.tile_width)) / m.tile_width;
                if ((x + (int)(w * m.tile_width)) % m.tile_width != 0)
                    j++;
                if (m.grid[i, j].ground == true)
                {
                    active = false;
                }
            }
            else
            {
                j = x / m.tile_width;
                if ((x) % m.tile_width != 0)
                    j--;
                if (j == -1)
                {
                    active = false;
                    return;
                }
                if (m.grid[i, j].ground == true)
                {
                    active = false;
                }
            }
            x += velx;
            if (x > m.lw * m.tile_width)
                active = false;
            if (x <= 0)
            {
                active = false;
                return;
            }
            //enemy collision
            if(m.P.ishit(x,y,(int)(w*m.tile_width), (int)(h * m.tile_height)) == true)
            {
                m.P.active = false;
                active = false;
            }
        }
    }
    class enemy2
    {
        public int x, y;
        public float w = 1f, h = 1f;
        public int velx = 0, vely = 0;
        public bool onground = true, left = false, right = false, active = true;
        public map m;
        public Random R = new Random();
        public int rand_seed = -1;
        public bool ishit(int x, int y, int w, int h)
        {
            if ((this.x <= x || this.x <= x + w) && x <= this.x + (int)(this.w * m.tile_width))
            {
                if ((this.y <= y || this.y <= y + h) && y <= this.y + (int)(this.h * m.tile_width))
                    return true;
            }
            return false;
        }
        void fire(char d)
        {
            enemy_bullet pnn = new enemy_bullet();
            int r, c;
            pnn.y = y + (int)(m.tile_height * 0.5);
            pnn.velx = (int)(m.tile_width * 0.5);
            c = x / m.tile_width;
            r = y / m.tile_height;
            if (x % m.tile_width != 0)
                c++;
            if (y % m.tile_height != 0)
                r++;
            if (d == 'r')
            {
                pnn.x = x + (int)(w * m.tile_width) + 10;
                if (m.grid[r, c + 1].ground == true)
                    return;
            }
            else
            {
                pnn.x = x - 10;
                pnn.velx *= -1;
                if (m.grid[r, c - 1].ground == true)
                    return;
            }
            pnn.m = m;
            m.eb.Add(pnn);
        }
        public void anim()
        {
            if(m.P.x > x)//hero at right
            {
                if((m.P.x-x)/m.tile_width <= m.visible_x/2)
                {//hero at half screen
                    right = true;
                    if(y/m.tile_height == m.P.y/m.tile_height)
                    {
                        if (rand_seed <= 0)
                        {
                            //fire
                            fire('r');
                            rand_seed = R.Next(500, 1000);
                        }
                        else
                        {
                            rand_seed -= R.Next(25, 75);
                        }
                    }
                }
            }
            if (m.P.x < x)//hero at left
            {
                if ((x-m.P.x) / m.tile_width <= m.visible_x / 2)
                {//hero at half screen
                    left = true;
                    if (y / m.tile_height == m.P.y / m.tile_height)
                    {
                        if (rand_seed <= 0)
                        {
                            //fire
                            fire('l');
                            rand_seed = R.Next(500, 1000);
                        }
                        else
                        {
                            rand_seed -= R.Next(25, 75);
                        }
                    }
                }
            }
            int i, j, j2, ny, nx;
            if (left == true)
            {
                velx = (-m.tile_width) / 5;
            }
            if (right == true)
            {
                velx = (m.tile_width) / 5;
            }
            ny = y + vely;
            nx = x + velx;

            if (vely >= 0)
            {
                //check = m.ishit(x, ny+m.tile_height);
                i = (ny) / m.tile_height;
                if (ny % m.tile_height != 0)
                    i++;
                j = nx / m.tile_width;
                if (nx % m.tile_width != 0)
                    j2 = j + 1;
                else
                    j2 = j;
                if (i < 0 || i + 1 >= m.lh || j < 0 || j2 < 0 || j > m.lw || j2 > m.lw)
                    return;
                if (m.grid[i + 1, j].ground == true || m.grid[i + 1, j2].ground == true || m.grid[i + 1, j].ladder == true || m.grid[i + 1, j2].ladder == true)
                {//found something
                    ny = (i) * m.tile_height;
                    vely = 0;
                    onground = true;
                }
            }
            if (velx < 0)
            {//left
                j = nx / m.tile_width;
                i = y / m.tile_height;
                if (i < 0 || i + 1 > m.lh || j - 1 < 0 || j > m.lw)
                    return;
                if (m.grid[i, j].ground == true || m.grid[i + 1, j].nothing == true)
                {//found something
                    nx = (j + 1) * m.tile_width;
                    velx = 0;
                    left = false;
                    right = true;
                }
            }
            else
            {//right
                j = nx / m.tile_width;
                i = y / m.tile_height;
                j2 = j;
                if (i < 0 || i > m.lh || j < 0 || j2 < 0 || j > m.lw - 1 || j2 > m.lw)
                    return;

                if (m.grid[i, j + 1].ground == true || m.grid[i + 1, j + 1].nothing == true)
                {//found something
                    nx = (j) * m.tile_width;
                    velx = 0;
                    right = false;
                    left = true;
                }
            }
            x = nx;
            y = ny;
            if (vely > 60)
                vely = 60;
            if (velx > 60)
                velx = 60;
            if (vely < -60)
                vely = -60;
            if (velx < -60)
                velx = -60;
            if (onground == false)
                vely += m.tile_height / 4;
            onground = false;
            //hero collision
            if (m.P.ishit(x, y, (int)(w * m.tile_width), (int)(h * m.tile_height)) == true)
            {
                m.P.active = false;
            }
        }
    }
    class enemy1
    {
        public int x, y;
        public float w=1f, h=1f;
        public int velx=0, vely=0;
        public bool onground=true,left=false,right=false,active=true;
        public map m;
        public bool ishit(int x, int y, int w, int h)
        {
            if ((this.x <= x || this.x <= x + w) && x <= this.x + (int)(this.w * m.tile_width))
            {
                if ((this.y <= y || this.y <= y + h) && y <= this.y + (int)(this.h * m.tile_width))
                    return true;
            }
            return false;
        }
        public void anim()
        {
            int i, j,j2,ny,nx;
            if (left == true)
            {
                velx = (-m.tile_width) / 5;
            }
            if (right == true)
            {
                velx = (m.tile_width) / 5;
            }
            ny = y + vely;
            nx = x + velx;
            
            if(vely>=0)
            {
                //check = m.ishit(x, ny+m.tile_height);
                i = (ny) / m.tile_height;
                if (ny % m.tile_height != 0)
                    i++;
                j = nx / m.tile_width;
                if (nx % m.tile_width != 0)
                    j2 = j + 1;
                else
                    j2 = j;
                if (i < 0 || i + 1 >= m.lh || j < 0 || j2 < 0 || j > m.lw || j2 > m.lw)
                    return;
                if (m.grid[i + 1, j].ground == true || m.grid[i + 1, j2].ground == true || m.grid[i + 1, j].ladder == true || m.grid[i + 1, j2].ladder == true)
                {//found something
                    ny = (i) * m.tile_height;
                    vely = 0;
                    onground = true;
                }
            }
            if (velx < 0)
            {//left
                j = nx / m.tile_width;
                i = y / m.tile_height;
                if (i < 0 || i+1 > m.lh || j-1 < 0  || j > m.lw)
                    return;
                if (m.grid[i, j].ground == true || m.grid[i+1,j].nothing == true)
                {//found something
                    nx = (j + 1) * m.tile_width;
                    velx = 0;
                    left = false;
                    right = true;
                }
            }
            else
            {//right
                j = nx / m.tile_width;
                i = y / m.tile_height;
                j2 = j;
                if (i < 0 || i > m.lh || j < 0 || j2 < 0 || j > m.lw - 1 || j2 > m.lw)
                    return;

                if (m.grid[i, j + 1].ground == true || m.grid[i + 1, j + 1].nothing == true)
                {//found something
                    nx = (j) * m.tile_width;
                    velx = 0;
                    right = false;
                    left = true;
                }
            }
            x = nx;
            y = ny;
            if (vely > 60)
                vely = 60;
            if (velx > 60)
                velx = 60;
            if (vely < -60)
                vely = -60;
            if (velx < -60)
                velx = -60;
            if (onground == false)
                vely += m.tile_height / 4;
            onground = false;
            //hero collision
            if (m.P.ishit(x, y, (int)(w * m.tile_width), (int)(h * m.tile_height)) == true)
            {
                m.P.active = false;
            }
            //goto 
        }
    }
    class bulllet
    {
        public int x, y,velx;
        public float w=0.5f, h=0.25f;
        public map m;
        public bool active = true;
        public void anim()
        {
            int i, j, j2;
            i = y / m.tile_height;
            
            if(velx > 0)
            {
                j = (x + (int)(w * m.tile_width)) / m.tile_width;
                if ((x + (int)(w * m.tile_width)) % m.tile_width != 0)
                    j++;
                if (m.grid[i,j].ground == true)
                {
                    active = false;
                }
            }
            else
            {
                j = x / m.tile_width;
                if ((x) % m.tile_width != 0)
                    j--;
                if (j == -1)
                {
                    active = false;
                    return;
                }
                if (m.grid[i, j].ground == true)
                {
                    active = false;
                }
            }
            x += velx;
            if(x > m.lw*m.tile_width)
                active = false;
            if (x <= 0)
            {
                active = false;
                return;
            }
            //enemy collision
            for(int e = 0; e < m.e1.Count; e++)
            {
                if( m.e1[e].ishit(x,y,(int)(w*m.tile_width), (int)(h * m.tile_height)) )
                {
                    m.e1[e].active = false;
                    active = false;
                }
            }
            for (int e = 0; e < m.e2.Count; e++)
            {
                if (m.e2[e].ishit(x, y, (int)(w * m.tile_width), (int)(h * m.tile_height)))
                {
                    m.e2[e].active = false;
                    active = false;
                }
            }
        }
    }
    class player
    {
        public int x,y;//remeber to difference the first 
        public float w = 1, h = 1;
        public int velx=0, vely=0;
        public char d='r',ld;
        public map m;
        public bool onground = false,left=false,right=false,active=true,laser=false;
        public int lX,lY,stx;
        public List<bulllet> b = new List<bulllet>();
        
        public void anim()
        {
            if(laser==true)
            {
                if (ld == 'r')
                {
                    lX += m.tile_width;
                    for (int e = stx; e <= lX; e+=m.tile_width)
                    {
                        for(int k=0;k<m.e1.Count;k++)
                        {
                            if (m.e1[k].x <= e && e <= m.e1[k].x+(int)(m.e1[k].w*m.tile_width) && m.e1[k].y <= lY && lY <= m.e1[k].y + (int)(m.e1[k].h * m.tile_height))
                            {
                                m.e1[k].active = false;
                            }
                        }
                        for (int k = 0; k < m.e2.Count; k++)
                        {
                            if (m.e2[k].x <= e && e <= m.e2[k].x + (int)(m.e2[k].w * m.tile_width))
                            {
                                m.e2[k].active = false;
                            }
                        }
                    }
                }
                else
                    lX -= m.tile_width;
            }
            if (left == true)
            {
                velx += (-m.tile_width) / 3;
                if(onground==false)
                    velx += (-m.tile_width) / 3;
            }
            if (right == true)
            {
                velx += (m.tile_width) / 3;
                if (onground == false)
                    velx += (m.tile_width) / 3;
            }
            for (int f = 0; f < b.Count; f++)
                b[f].anim();
            int nx=x+velx, ny=y+vely,i,i2,j,j2;
            j = nx / m.tile_width;
            if (nx % m.tile_width != 0)
                j2 = j + 1;
            else
                j2 = j;
            i = ny / m.tile_height;
            if (ny % m.tile_height != 0)
                i++;
            if (i < 0 || i > m.lh || j < 0 || j2 < 0 || j > m.lw || j2 > m.lw)
                return;
            if ((m.grid[i, j].ele == true || m.grid[i, j2].ele == true) && (m.grid[i+1,j].ground==false))
            {
                y = ny;
                x = nx;
                velx = 0;
                if(vely < 0)
                {
                    if (m.grid[i + 1, j].ground == true)
                        vely = 0;
                }
                else
                {
                    if (m.grid[i - 1, j].ground == true)
                        vely = 0;
                }
                return;
            }
            if (m.grid[i,j].ladder == true || m.grid[i, j2].ladder == true)
            {
                y = ny;
                x = nx;
                velx = 0;
                vely = 0;
                return;
            }

            onground = false;
            if (velx < 0)
            {//left
                j = nx / m.tile_width;
                i = ny / m.tile_height;
                if (ny % m.tile_height != 0)
                    i2 = i + 1;
                else
                    i2 = i;
                if (i < 0 || i > m.lh || j < 0 || j > m.lw || i2 > m.lh)
                    return;
                if (m.grid[i, j].ground == true || m.grid[i2, j].ground == true)
                {//found something
                    nx = (j + 1) * m.tile_width;
                    velx = -velx/2;
                    left = false;
                }
            }
            else
            {//right
                j = nx / m.tile_width;
                i = ny / m.tile_height;
                if (nx % m.tile_width != 0)
                    j2 = j + 1;
                else
                    j2 = j;
                if (ny % m.tile_height != 0)
                    i2 = i + 1;
                else
                    i2 = i;
                if (i < 0 || i > m.lh || j < 0 || j2 < 0 || j > m.lw - 1 || j2 > m.lw || i2 > m.lh)
                    return;

                if (m.grid[i, j + 1].ground == true || m.grid[i, j2].ground == true)
                {//found something
                    nx = (j) * m.tile_width;
                    velx = 0;
                    right = false;
                }
            }
            if (vely < 0)
            {//up
                i = y / m.tile_height;
                if (y % m.tile_height != 0)
                    i++;
                j = x / m.tile_width;
                if (x % m.tile_width != 0)
                    j2 = j + 1;
                else
                    j2 = j;
                if(m.grid[i-1,j].ground == true || m.grid[i - 1, j2].ground == true)
                {
                    ny = (i) * m.tile_height;
                    vely = 0;
                    return;
                }
                onground = false;
                i = (ny) / m.tile_height;
                if (ny % m.tile_height != 0)
                    i++;
                j = nx / m.tile_width;
                if (nx % m.tile_width != 0)
                    j2 = j + 1;
                else
                    j2 = j;
                if (m.grid[i - 1, j].ground == true || m.grid[i - 1, j2].ground == true)
                {//found something
                    ny = (i) * m.tile_height;
                    vely = 0;
                }
            }
            else 
            {//down
                //check = m.ishit(x, ny+m.tile_height);
                i = (ny) / m.tile_height;
                if (ny % m.tile_height != 0)
                    i2 = i + 1;
                else
                    i2 = i;
                j = nx / m.tile_width;
                if (nx % m.tile_width != 0)
                    j2 = j + 1;
                else
                    j2 = j;
                if (i < 0 || i > m.lh || i2 > m.lh || j < 0 || j2 < 0 || j > m.lw || j2 > m.lw)
                    return;
                if (m.grid[i2, j2].ground == true || m.grid[i2, j].ground == true)
                {
                    ny = (i2-1) * m.tile_height;
                    vely = 0;
                    onground = true;
                }
                if (m.grid[i+1,j].ground == true || m.grid[i+1, j2].ground == true)
                {//found something
                    ny = (i) * m.tile_height;
                    vely = 0;
                    onground = true;
                }
            }
            x = nx;
            y = ny;
            velx = 0;
            if (vely > 60)
                vely = 60;
            if(velx > 60)
                velx = 60;
            if (vely < -60)
                vely = -60;
            if (velx < -60)
                velx = -60;
            if (onground==false)
                vely += m.tile_height/4;
            
        }
        public bool ishit(int x, int y, int w, int h) 
        {
            if((this.x <= x || this.x <= x + w) && x <= this.x + (int)(this.w * m.tile_width))
            {
                if((this.y <= y || this.y <= y + h) && y <= this.y + (int)(this.h * m.tile_width))
                        return true;
            }
            return false;
        }
    }
    class tile
    {
        public bool ground = false, nothing = false, ladder = false,ele =false;
    }
    class map
    {
        public tile[,] grid;
        public int lw, lh;
        public int tile_width, tile_height;
        public player P;
        public List<enemy1> e1 = new List<enemy1>();
        public List<enemy2> e2 = new List<enemy2>();
        public List<enemy_bullet> eb = new List<enemy_bullet>();
        public int camera_x,camera_y,visible_x,visible_y;
        public map(int lw,int lh,player p,Bitmap level,int tile_width,int tile_height)
        {
            this.P = p;
            this.tile_width = tile_width;
            this.tile_height = tile_height;
            this.lh = lh;//lh
            this.lw = lw;//lw
            grid = new tile[lh, lw];
            string[] m = new string[lh];
                //1 enemy1  2 enemy2    g ground0   G fround1   L Ladder    e elevator  b border
                m[0]=  ".........................................................................................b";
                m[1]=  ".........................................................................................b";
                m[2]=  ".........................................................................................b";
                m[3]=  ".........................gggggggLgggg....................................................b";
                m[4]=  ".....................1..........L........................................................b";
                m[5]=  "................................L........................................................b";
                m[6]=  "................................L........................................................b";
                m[7]=  "...................GGGGGGGGGGGGGGGGGGGGGGGGGGGeGGGGGGGGG....GGGGGGGGGGGGGGGGGeGGGGGGGGGGGb";
                m[8]=  "..............1...GG..........................e..............................e...........b";
                m[9]=  ".....G..M...G....GGG..........................e..............................e...........b";
                m[10]= "GGG.GGGGGGGGGGGGGGGG..........................e..............................e...........b";
                m[11]= "..G.G.........................................e..............................e...........b";
                m[12]= "..G.GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG........e..............................e...........b";
                m[13]= "..G.........................2........G........e..............................e...........b";
                m[14]= "..GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGeGGG........e..............................e...........b";
                m[15]= "..................................e...........e..............................e...........b";
                m[16]= "..................................e...........e..............................e...........b";
                m[17]= "..................................e...........e..............................e...........b";
                m[18]= "..................................e...........e..............................e...........b";
                m[19]= "..................................e...........e..............................e...........b";
                m[20] ="..................................e...........e..............................e...........b";
                m[21] ="..................................e...........e..............................e...........b";
                m[22] ="..................................eGGGGGGGGGGGe..............................e...........b";
                m[23] ="..............................................e..............................e...........b";
                m[24] ="..............................................e..............................e...........b";
                m[25] ="..............................................e..............................e...........b";
                m[26] ="..............................................e..............................e...........b";
                m[27] ="..............................................e..............................e...........b";
                m[28] ="..............................................e..............................e...........b";
                m[29] ="bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";
            Graphics G = Graphics.FromImage(level);
            //set level bg
            Bitmap bg = new Bitmap("Background.png");
            G.DrawImage(bg , new Rectangle(0, 0, level.Width, level.Height), new Rectangle(0,0,bg.Width,bg.Height), GraphicsUnit.Pixel);
            //
            List<Bitmap> assets = new List<Bitmap>();
            assets.Add(new Bitmap("g1.png"));
            assets.Add(new Bitmap("g2.png"));
            assets.Add(new Bitmap("l1.png"));
            assets[2].MakeTransparent(Color.White);
            assets.Add(new Bitmap("b.png"));
            for (int i = 0,y = 0; i < 30; i++,y+=tile_height)
            {
                for (int j = 0,x = 0; j < 90; j++,x += tile_width)
                {
                    grid[i, j] = new tile();
                    if(m[i][j]=='G')
                    {
                        grid[i, j].ground = true;
                        G.DrawImage(assets[0], new Rectangle(x, y, tile_width, tile_height), new Rectangle(0, 0, assets[0].Width, assets[0].Height), GraphicsUnit.Pixel);
                    }
                    else if (m[i][j] == 'g')
                    {
                        grid[i, j].ground = true;
                        G.DrawImage(assets[1], new Rectangle(x, y, tile_width, tile_height), new Rectangle(0, 0, assets[1].Width, assets[1].Height), GraphicsUnit.Pixel);
                    }
                    else if (m[i][j] == 'L')
                    {
                        grid[i, j].ladder = true;
                        if(m[i][j-1]=='g' || m[i][j + 1] == 'g')
                            G.DrawImage(assets[1], new Rectangle(x, y, tile_width, tile_height), new Rectangle(0, 0, assets[1].Width, assets[1].Height), GraphicsUnit.Pixel);
                        if (m[i][j - 1] == 'G' || m[i][j + 1] == 'G')
                            G.DrawImage(assets[0], new Rectangle(x, y, tile_width, tile_height), new Rectangle(0, 0, assets[0].Width, assets[0].Height), GraphicsUnit.Pixel);
                        G.DrawImage(assets[2],new Rectangle(x, y, tile_width, tile_height),new Rectangle(0,0,assets[2].Width, assets[2].Height),GraphicsUnit.Pixel);
                    }
                    else if (m[i][j]=='b')
                    {
                        grid[i, j].ground = true;
                        G.DrawImage(assets[3], new Rectangle(x, y, tile_width, tile_height), new Rectangle(0, 0, assets[3].Width, assets[3].Height), GraphicsUnit.Pixel);
                    }
                    else if(m[i][j]=='e')
                    {
                        grid[i, j].ele = true;
                        Color e = Color.FromArgb(100, Color.Blue);
                        G.FillRectangle(new SolidBrush(e), x, y, tile_width, tile_height);
                    }
                    else
                    {
                        if (m[i][j] == 'M')
                        {
                            //MessageBox.Show("found m at " + i + " " + j);
                            p.x = x;
                            p.y = y;
                        }
                        else if(m[i][j] == '1')
                        {
                            enemy1 pnn = new enemy1();
                            pnn.x = x;
                            pnn.y = y;
                            pnn.left = true;
                            pnn.right = false;
                            pnn.m = this;
                            e1.Add(pnn);
                        }
                        else if (m[i][j] == '2')
                        {
                            enemy2 pnn = new enemy2();
                            pnn.x = x;
                            pnn.y = y;
                            pnn.m = this;
                            e2.Add(pnn);
                        }
                        grid[i, j].nothing = true;
                    }
                }
            }
        }
    }
    public partial class Form1 : Form
    {
        int visible_x = 30,visible_y = 10;
        int tile_width, tile_height;
        map M;
        Bitmap img,level;
        Graphics G;
        Timer T = new Timer();
        player P;
        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Load += Form1_Load;
            T.Tick += loop;
            T.Interval = 100;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Left:
                    P.left = false;
                    break;
                case Keys.Right:
                    P.right = false;
                    break;
                case Keys.L:
                    P.laser = true;
                    P.lY = P.y + (int)(tile_height * 0.5); ;
                    P.ld = P.d;
                    if(P.d == 'r')
                    {
                        P.stx = P.x + (int)(P.w * tile_width) + tile_width / 4;
                    }
                    else
                    {
                        P.stx = P.x - tile_width / 4;
                    }
                    P.lX = P.stx;
                    break;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int i, j, j2;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    P.d = 'l';
                    P.left = true;
                    P.right = false;
                    break;
                case Keys.Right:
                    P.d = 'r';
                    P.left = false;
                    P.right = true;
                    break;
                case Keys.Space:
                    if(P.vely == 0 && P.onground == true)
                        P.vely = (-2*tile_height)/2;
                    break;
                case Keys.Down:
                    i = P.y / tile_height;
                    if (P.y % tile_height != 0)
                        i++;
                    j = P.x / tile_width;
                    if (P.x % tile_width != 0)
                        j2 = j + 1;
                    else
                        j2 = j;
                    if((M.grid[i,j].ladder == true || M.grid[i, j2].ladder == true || M.grid[i, j].ele == true || M.grid[i, j2].ele == true) && (M.grid[i+1,j].ground==false && M.grid[i + 1, j2].ground == false))
                        P.vely += tile_height / 3;
                    break;
                case Keys.Up:
                    if(P.vely == 0)
                        P.vely = -tile_height / 3;
                    break;
                case Keys.F:
                    bulllet pnn = new bulllet();
                    pnn.y = P.y + (int)(tile_height * 0.5);
                    pnn.velx = (int)(M.tile_width * 0.5);
                    j = P.x / tile_width;
                    i = P.y / tile_height;
                    if (P.x % tile_width != 0)
                        j++;
                    if (P.y % tile_height != 0)
                        i++;
                    if (P.d == 'r')
                    {
                        pnn.x = P.x + (int)(P.w * tile_width) + 10;
                        if (M.grid[i, j+1].ground == true)
                            break;
                    }
                    else
                    {
                        pnn.x = P.x - 10;
                        pnn.velx *= -1;
                        if (M.grid[i, j - 1].ground == true)
                            break;
                    }
                    pnn.m = M;
                    P.b.Add(pnn);
                    break;

            }
        }

        private void loop(object sender, EventArgs e)
        {
            if (P.active == false)
                return;
            P.anim();
            draw(this.CreateGraphics());
            for (int i = 0; i < M.eb.Count; i++)
            {
                M.eb[i].anim();
            }
            for (int i=0;i<M.e1.Count;i++)
            {
                M.e1[i].anim();
            }
            for (int i = 0; i < M.e2.Count; i++)
            {
                M.e2[i].anim();
            }
            draw(this.CreateGraphics());
            if (P.active == false)
            {
                MessageBox.Show("game over");
                T.Stop();
                return;
            }
            this.Text = P.x + " " + P.y + " " + P.velx + " " + P.vely + " " + P.onground + " " + P.b.Count;
            for(int i = 0; i < P.b.Count; i++)
            {
                if(P.b[i].active == false)
                {
                    P.b.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < M.e1.Count; i++)
            {
                if(M.e1[i].active == false)
                {
                    M.e1.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < M.e2.Count; i++)
            {
                if (M.e2[i].active == false)
                {
                    M.e2.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < M.eb.Count; i++)
            {
                if (M.eb[i].active == false)
                {
                    M.eb.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int lw = 90, lh = 30;//3*visible_x
            tile_width = ClientSize.Width/visible_x;
            tile_height = ClientSize.Height/visible_y;
            img = new Bitmap(ClientSize.Width,ClientSize.Height);
            G = Graphics.FromImage(img);
            P = new player();
            level = new Bitmap(tile_width*lw,tile_height*lh);
            M = new map(lw,lh,P,level,tile_width,tile_height);
            P.m = M;
            M.camera_x = 0;
            M.camera_y = 0;
            M.visible_x = visible_x;
            M.visible_y = visible_y;
            T.Start();
        }
        void draw(Graphics g)
        {
            int camerax = P.x - (visible_x/2)*tile_width;
            int cameraY = P.y - (visible_y/2)*tile_width;
            if(camerax < 0)
                camerax = 0;
            if(cameraY < 0)
                cameraY = 0;
            if (camerax > (M.lw)*tile_width - visible_x*tile_width)
                camerax = (M.lw) * tile_width - visible_x*tile_width;
            if(cameraY > M.lh*tile_height - visible_y * tile_height)
                cameraY = M.lh * tile_height - visible_y * tile_height;

            G.DrawImage(level, new Rectangle(0, 0, tile_width * visible_x, tile_height * visible_y), new Rectangle(camerax, cameraY, tile_width * visible_x, tile_height * visible_y), GraphicsUnit.Pixel);
            for (int i = 0; i < M.eb.Count; i++)
            {
                G.FillRectangle(new SolidBrush(Color.Red), M.eb[i].x - camerax, M.eb[i].y - cameraY, M.eb[i].w * tile_width, M.eb[i].h * tile_height);
            }
            //player
            G.FillRectangle(new SolidBrush(Color.Violet), P.x-camerax, P.y - cameraY, P.w*tile_width, P.h*tile_height);
            if (P.laser == true)
            {
                if (P.ld == 'r')
                    G.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Red)), P.stx-camerax, P.lY-cameraY, P.lX - P.stx, tile_height / 4);
                else
                    G.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Red)), P.lX, P.lY,P.stx-P.lX, tile_height / 4);
                if (P.lX > camerax + visible_x * tile_width || P.lX < camerax)
                    P.laser = false;
            }
            for (int i = 0; i < M.e1.Count; i++)
            {
                G.FillRectangle(new SolidBrush(Color.Blue), M.e1[i].x - camerax, M.e1[i].y - cameraY, M.e1[i].w * tile_width, M.e1[i].h * tile_height);
            }
            for (int i = 0; i < M.e2.Count; i++)
            {
                G.FillRectangle(new SolidBrush(Color.Red), M.e2[i].x - camerax, M.e2[i].y - cameraY, M.e2[i].w * tile_width, M.e2[i].h * tile_height);
            }
            for (int i = 0; i < P.b.Count; i++)
            {
                G.FillRectangle(new SolidBrush(Color.Orange), P.b[i].x - camerax, P.b[i].y - cameraY, P.b[i].w * tile_width, P.b[i].h * tile_height);
            }
            g.DrawImage(img, new Rectangle(0,0,ClientSize.Width+50,ClientSize.Height),new Rectangle(0,0,img.Width,img.Height),GraphicsUnit.Pixel);

            M.camera_x = camerax;
            M.camera_y = cameraY;
        }
    }
}
