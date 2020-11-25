import React, { useState, useEffect, Component } from 'react';
import _ from 'lodash';
import { Container, Row, Col, Button, Card, CardBody, CardTitle, Modal, ModalHeader, ModalBody, ModalFooter, Media, Table } from "reactstrap";
import Breadcrumbs from '../../components/Common/Breadcrumb';
import { withRouter, Link } from 'react-router-dom';
import { withTranslation } from 'react-i18next';
import stateWrapper from "../../containers/provider";

class Competition extends Component {
    constructor(props) {
        super(props);
        this.state = {

        }
    }

    render() {
        return (
            <React.Fragment>
                <div className="page-content">
                    <Container fluid>
                        <Breadcrumbs title={this.props.t('competitions')} />
                    </Container>
                </div>
            </React.Fragment>
        )
    }
}

export default withRouter(withTranslation()(stateWrapper(Competition)))
