/*
 * Copyright 2008-2009 LinkedIn, Inc
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

package voldemort.server.rebalance;

import org.junit.Test;
import voldemort.client.rebalance.RebalancePartitionsInfo;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static org.junit.Assert.*;

/**
 * Test for {@link RebalancerState}
 */
public class RebalancerStateTest {

    @Test
    public void testToJson() {
        List<RebalancePartitionsInfo> rebalancePartitionsInfos = Arrays.asList(
                       new RebalancePartitionsInfo(2,
                                                   0,
                                                   Arrays.asList(1, 2, 3, 4),
                                                   new ArrayList<Integer>(0),
                                                   Arrays.asList("test1", "test2"),
                                                   0),
                       new RebalancePartitionsInfo(3,
                                                   1,
                                                   Arrays.asList(5, 6, 7, 8),
                                                   new ArrayList<Integer>(0),
                                                   Arrays.asList("test1", "test2"),
                                                   0));

        RebalancerState in = new RebalancerState(rebalancePartitionsInfos);
        String jsonIn = in.toJsonString();
        RebalancerState out = RebalancerState.create(jsonIn);
        assertEquals(in, out);

        String jsonOut = out.toJsonString();
        assertEquals(jsonIn, jsonOut);
        
    }
}
