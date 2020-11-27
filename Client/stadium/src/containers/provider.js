import React from "react";
import { Subscribe } from "unstated";
import allStores from "./index";

export default WrappedComponent => {
  
  const subscribeWrapper = ({ ...props }) => (

    <Subscribe to={allStores}>
        {(
          layoutStore,
          footballStore,
        ) => (
          <WrappedComponent
            layoutStore={layoutStore}
            footballStore={footballStore}
            {...props}
          />
        )}
    </Subscribe>

  )

  return subscribeWrapper;
};
