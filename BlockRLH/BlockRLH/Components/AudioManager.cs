using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using BlockRLH;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

class AudioManager : GameComponent
{
    private Song[] song;
    private const int SONG_NOT_CHOSEN = -1;
    private int SecondLastSong;
    private int LastSong;
    private Random rand;

    private KeyboardState NewState;
    private KeyboardState OldState;


    public AudioManager(Game game) : base(game)
    {
        MediaPlayer.Volume = .5f;

        song = new Song[6];
        rand = new Random();
        LastSong = SONG_NOT_CHOSEN;
        SecondLastSong = SONG_NOT_CHOSEN;

        song[0] = game.Content.Load<Song>(@"Music/Piano Song");
        song[1] = game.Content.Load<Song>(@"Music/Real Life");
        song[2] = game.Content.Load<Song>(@"Music/Orkistroll");
        song[3] = game.Content.Load<Song>(@"Music/Guitar and Violin");
        song[4] = game.Content.Load<Song>(@"Music/Technotic Vibe");
        song[5] = game.Content.Load<Song>(@"Music/Diana Jones");

        UpdateMusic();
    }

    public override void Update(GameTime gameTime)
    {
        UpdateMusic();
        UpdateKeys();
    }

    private void UpdateKeys()
    {
        OldState = NewState;
        NewState = Keyboard.GetState();

        if (NewState.IsKeyDown(Keys.OemOpenBrackets) && !(OldState.IsKeyDown(Keys.OemOpenBrackets)))
        {
            MediaPlayer.Stop();
        }
    }

    private void UpdateMusic()
    {
        if (MediaPlayer.State == MediaState.Stopped)
        {
            int NextSong = SONG_NOT_CHOSEN;

            do
            {
                NextSong = rand.Next(song.Length);
            } while (NextSong == LastSong || NextSong == SecondLastSong);

            SecondLastSong = LastSong;
            LastSong = NextSong;
            MediaPlayer.Play(song[NextSong]);
        }
    }
}