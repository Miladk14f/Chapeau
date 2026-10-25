package nl.inholland.quizgame.state;

public class PlayingState implements AppState {

    @Override
    public String getName() { return "PLAYING"; }

    @Override
    public boolean canLoadQuiz() { return false; }

    @Override
    public boolean canStartQuiz(boolean quizLoaded) { return false; }

    @Override
    public boolean isPlaying() { return true; }
}
