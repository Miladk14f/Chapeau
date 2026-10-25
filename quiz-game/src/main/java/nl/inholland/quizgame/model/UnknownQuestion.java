package nl.inholland.quizgame.model;

import java.util.List;

/**
 * Placeholder for question types the application does not recognise.
 * Satisfies robustness requirement: unrecognised types do not crash the app.
 */
public class UnknownQuestion extends Question {

    private final String originalType;

    public UnknownQuestion(String name, String title, boolean isRequired, int timeLimit, String originalType) {
        super(name, title, isRequired, timeLimit);
        this.originalType = originalType;
    }

    @Override
    protected boolean checkAnswer(String answer) {
        return false; // unknown questions are always skipped, never scored
    }

    @Override
    public List<String> getChoices() {
        return List.of();
    }

    @Override
    public String getQuestionType() {
        return "unknown:" + originalType;
    }

    public String getOriginalType() { return originalType; }
}
