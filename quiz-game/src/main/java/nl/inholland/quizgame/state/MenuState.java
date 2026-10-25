package nl.inholland.quizgame.state;

public class MenuState implements AppState {

    @Override
    public String getName() { return "MENU"; }

    @Override
    public boolean canLoadQuiz() { return true; }

    @Override
    public boolean canStartQuiz(boolean quizLoaded) { return quizLoaded; }

    @Override
    public boolean isPlaying() { return false; }
}
