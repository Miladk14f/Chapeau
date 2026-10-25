package nl.inholland.quizgame.factory;

import com.fasterxml.jackson.databind.JsonNode;
import nl.inholland.quizgame.model.*;

import java.util.ArrayList;
import java.util.List;

/**
 * Factory pattern: one place creates Question subclasses from JSON.
 * Adding a new question type requires only a new class and one case here.
 */
public class QuestionFactory {

    private QuestionFactory() {}

    public static Question create(JsonNode node, int timeLimit) {
        String type = node.path("type").asText("");
        String name = node.path("name").asText("unknown");
        String title = node.path("title").asText("");
        boolean isRequired = node.path("isRequired").asBoolean(false);

        return switch (type) {
            case "radiogroup" -> buildRadioGroup(node, name, title, isRequired, timeLimit);
            case "boolean"    -> buildBoolean(node, name, title, isRequired, timeLimit);
            default           -> new UnknownQuestion(name, title, isRequired, timeLimit, type);
        };
    }

    private static RadioGroupQuestion buildRadioGroup(JsonNode node, String name, String title,
                                                       boolean isRequired, int timeLimit) {
        List<String> choices = new ArrayList<>();
        JsonNode choicesNode = node.path("choices");
        if (choicesNode.isArray()) {
            for (JsonNode c : choicesNode) {
                choices.add(c.asText());
            }
        }
        String correctAnswer = node.path("correctAnswer").asText("");
        String choicesOrder = node.path("choicesOrder").asText("none");
        return new RadioGroupQuestion(name, title, isRequired, timeLimit, choices, correctAnswer, choicesOrder);
    }

    private static BooleanQuestion buildBoolean(JsonNode node, String name, String title,
                                                 boolean isRequired, int timeLimit) {
        JsonNode correctNode = node.path("correctAnswer");
        boolean correct = correctNode.isBoolean() ? correctNode.asBoolean() : Boolean.parseBoolean(correctNode.asText("false"));
        String labelTrue = node.path("labelTrue").asText("True");
        String labelFalse = node.path("labelFalse").asText("False");
        return new BooleanQuestion(name, title, isRequired, timeLimit, correct, labelTrue, labelFalse);
    }
}
