package nl.inholland.quizgame.model;

import java.util.List;

public class BooleanQuestion extends Question {

    private final boolean correctAnswer;
    private final String labelTrue;
    private final String labelFalse;

    public BooleanQuestion(String name, String title, boolean isRequired, int timeLimit,
                           boolean correctAnswer, String labelTrue, String labelFalse) {
        super(name, title, isRequired, timeLimit);
        this.correctAnswer = correctAnswer;
        this.labelTrue = labelTrue != null ? labelTrue : "True";
        this.labelFalse = labelFalse != null ? labelFalse : "False";
    }

    @Override
    protected boolean checkAnswer(String answer) {
        boolean playerChoice = labelTrue.equalsIgnoreCase(answer.trim());
        return playerChoice == correctAnswer;
    }

    @Override
    public List<String> getChoices() {
        return List.of(labelTrue, labelFalse);
    }

    @Override
    public String getQuestionType() {
        return "boolean";
    }

    public boolean getCorrectAnswer() { return correctAnswer; }
    public String getLabelTrue() { return labelTrue; }
    public String getLabelFalse() { return labelFalse; }
}
