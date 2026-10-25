package nl.inholland.quizgame.state;

public class ResultsState implements AppState {

    @Override
    public String getName() { return "RESULTS"; }

    @Override
    public boolean canLoadQuiz() { return false; }

    @Override
    public boolean canStartQuiz(boolean quizLoaded) { return false; }

    @Override
    public boolean isPlaying() { return false; }
}
