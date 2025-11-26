package org.harvey.mongodb.util;

import java.util.Objects;
import java.util.StringJoiner;

/**
 * 构造字段形如school.students.1023.grade.math
 *
 * @author <a href="mailto:harvey.blocks@outlook.com">Harvey Blocks</a>
 * @version 1.0
 * @date 2025-11-19 19:08
 */
public class FieldPath {
    private final StringJoiner joiner = new StringJoiner(".");

    public FieldPath(String... fields) {
        if (fields == null || fields.length == 0) {
            throw new IllegalArgumentException("require at least one field to build path");
        }
        for (String field : fields) {
            get(field);
        }
    }


    public FieldPath get(int index) {
        joiner.add(String.valueOf(index));
        return this;
    }

    public FieldPath get(String inner) {
        Objects.requireNonNull(inner);
        joiner.add(inner);
        return this;
    }

    public String build() {
        return joiner.toString();
    }

    public String buildReference() {
        return "$" + joiner;
    }

    public static String refer(String... field) {
        return new FieldPath(field).buildReference();
    }
    public static String of(String... field) {
        return new FieldPath(field).build();
    }
}
