package nl.inholland.quizgame.model;

import java.util.List;

/**
 * Abstract base class for all question types — Template Method pattern.
 * Defines the skeleton: how a question is identified, timed, and checked.
 * Subclasses provide the type-specific steps.
 */
public abstract class Question {

    protected String name;
    protected String title;
    protected boolean isRequired;
    protected int timeLimit;

    protected Question(String name, String title, boolean isRequired, int timeLimit) {
        this.name = name;
        this.title = title;
        this.isRequired = isRequired;
        this.timeLimit = timeLimit;
    }

    // Template Method: fixed skeleton used by GameController
    public final boolean evaluate(String playerAnswer) {
        if (playerAnswer == null || playerAnswer.isBlank()) {
            return false;
        }
        return checkAnswer(playerAnswer);
    }

    // Steps that subclasses must implement
    protected abstract boolean checkAnswer(String answer);

    public abstract List<String> getChoices();

    public abstract String getQuestionType();

    // Common getters
    public String getName() { return name; }
    public String getTitle() { return title; }
    public boolean isRequired() { return isRequired; }
    public int getTimeLimit() { return timeLimit; }
}
