package nl.inholland.quizgame.manager;

import javafx.beans.property.IntegerProperty;
import javafx.beans.property.SimpleIntegerProperty;
import nl.inholland.quizgame.model.Quiz;
import nl.inholland.quizgame.state.AppState;
import nl.inholland.quizgame.state.MenuState;

/**
 * Singleton pattern: one shared game state visible to all screens.
 * Observer pattern: scoreProperty is a JavaFX property; the score label
 * in the view binds to it and updates automatically when the score changes.
 */
public class GameManager {

    private static GameManager instance;

    private String playerName;
    private Quiz currentQuiz;
    private int currentQuestionIndex;
    private final IntegerProperty scoreProperty = new SimpleIntegerProperty(0);
    private AppState currentState;

    private GameManager() {
        currentState = new MenuState();
    }

    public static GameManager getInstance() {
        if (instance == null) {
            instance = new GameManager();
        }
        return instance;
    }

    public void startGame(String playerName) {
        this.playerName = playerName;
        this.currentQuestionIndex = 0;
        this.scoreProperty.set(0);
    }

    public void incrementScore() {
        scoreProperty.set(scoreProperty.get() + 1);
    }

    public void nextQuestion() {
        currentQuestionIndex++;
    }

    public boolean hasNextQuestion() {
        return currentQuiz != null && currentQuestionIndex < currentQuiz.getQuestionCount() - 1;
    }

    public boolean isQuizFinished() {
        return currentQuiz == null || currentQuestionIndex >= currentQuiz.getQuestionCount();
    }

    // Getters and setters
    public String getPlayerName() { return playerName; }
    public Quiz getCurrentQuiz() { return currentQuiz; }
    public void setCurrentQuiz(Quiz quiz) { this.currentQuiz = quiz; }
    public int getCurrentQuestionIndex() { return currentQuestionIndex; }
    public int getScore() { return scoreProperty.get(); }
    public IntegerProperty scoreProperty() { return scoreProperty; }
    public AppState getCurrentState() { return currentState; }
    public void setState(AppState state) { this.currentState = state; }
}
