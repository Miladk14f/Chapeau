package nl.inholland.quizgame.model;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class RadioGroupQuestion extends Question {

    private final List<String> choices;
    private final String correctAnswer;
    private final String choicesOrder;

    public RadioGroupQuestion(String name, String title, boolean isRequired, int timeLimit,
                              List<String> choices, String correctAnswer, String choicesOrder) {
        super(name, title, isRequired, timeLimit);
        this.choices = new ArrayList<>(choices);
        this.correctAnswer = correctAnswer;
        this.choicesOrder = choicesOrder;
    }

    @Override
    protected boolean checkAnswer(String answer) {
        return correctAnswer != null && correctAnswer.equalsIgnoreCase(answer.trim());
    }

    @Override
    public List<String> getChoices() {
        List<String> result = new ArrayList<>(choices);
        if ("random".equalsIgnoreCase(choicesOrder)) {
            Collections.shuffle(result);
        }
        return result;
    }

    @Override
    public String getQuestionType() {
        return "radiogroup";
    }

    public String getCorrectAnswer() { return correctAnswer; }
}
