package org.harvey.mongodb.util;

import org.bson.Document;
import org.bson.conversions.Bson;

import java.util.Map;

/**
 * <p>
 * 封装了构造器{@link Document#Document(Map)}, 能更自由更规范地构建Document
 * </p><p>
 * 同时对文档形如<pre>{@code
 *    // language=Json
 *    "{"+
 *      "\"$gt\": 12,"+
 *      "\"$lt\": 15"+
 *    "}"
 *   }</pre>进行了一定增强
 * </p>
 *
 * @author <a href="mailto:harvey.blocks@outlook.com">Harvey Blocks</a>
 * @version 1.0
 * @date 2025-11-19 19:08
 */
public class ExpressionDocument extends Document {
    private ExpressionDocument() {
    }

    private ExpressionDocument(String key, Object value) {
        super(key, value);
    }

    private ExpressionDocument(Map<String, ?> map) {
        super(map);
    }

    public ExpressionDocument avg(Object value) {
        return append(Key.AVG, value);
    }

    public ExpressionDocument ne(Object value) {
        return append(Key.NE, value);
    }

    public ExpressionDocument eq(Object value) {
        return append(Key.EQ, value);
    }

    public ExpressionDocument lt(Object value) {
        return append(Key.LT, value);
    }

    public ExpressionDocument gt(Object value) {
        return append(Key.GT, value);
    }

    public ExpressionDocument lte(Object value) {
        return append(Key.LTE, value);
    }

    public ExpressionDocument gte(Object value) {
        return append(Key.GTE, value);
    }

    public ExpressionDocument max(Object value) {
        return append(Key.MAX, value);
    }

    public ExpressionDocument min(Object value) {
        return append(Key.MIN, value);
    }

    public Object first(Object value) {
        return append(Key.FIRST, value);
    }

    public ExpressionDocument append(Key key, Object value) {
        return append(key.field, value);
    }

    public ExpressionDocument append(String key, Object value) {
        super.append(key, value);
        return this;
    }


    public static ExpressionDocument of() {
        return new ExpressionDocument();
    }

    public static ExpressionDocument of(String key, Object value) {
        return new ExpressionDocument(key, value);
    }

    public static ExpressionDocument of(Key key, Object value) {
        return new ExpressionDocument(key.field, value);
    }

    public static ExpressionDocument of(Map<String, ?> map) {
        return new ExpressionDocument(map);
    }

    public static Bson empty() {
        return new Document();
    }


    public enum Key {
        NE("$ne"),
        EQ("$eq"),
        GT("$gt"),
        LT("$lt"),
        GTE("$gte"),
        LTE("$lte"),
        MAX("$max"),
        MIN("$min"),
        AVG("$avg"),
        FIRST("$first");
        private final String field;

        Key(String field) {this.field = field;}
    }

}
