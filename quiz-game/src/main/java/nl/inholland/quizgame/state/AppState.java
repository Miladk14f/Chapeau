package nl.inholland.quizgame.state;

/**
 * State pattern: the application behaves differently depending on which
 * state it is in. Controls what is enabled, what the timer does, and
 * whether results are saved.
 */
public interface AppState {
    String getName();
    boolean canLoadQuiz();
    boolean canStartQuiz(boolean quizLoaded);
    boolean isPlaying();
}
