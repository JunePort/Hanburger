package org.harvey.mongodb.util;

import org.bson.conversions.Bson;

import java.util.ArrayList;
import java.util.List;

/**
 * 构建Pipeline的构建类, 用于链式编程
 *
 * @author <a href="mailto:harvey.blocks@outlook.com">Harvey Blocks</a>
 * @version 1.0
 * @date 2025-11-20 22:16
 */
public class AggregatePipelineBuilder {
    private final List<Bson> pipeline = new ArrayList<>();

    public AggregatePipelineBuilder append(Bson operator) {
        pipeline.add(operator);
        return this;
    }

    public List<Bson> build() {
        return pipeline;
    }
}
