import React, { useState, useEffect, useRef } from 'react';
import _ from 'lodash';
import classnames from 'classnames';
import { LazyLoadImage } from 'react-lazy-load-image-component';
import { Container, Row, Col, Badge, Button, Card, CardBody, CardTitle, NavItem, NavLink, Nav, Modal, ModalHeader, ModalBody, ModalFooter, Media, Table } from "reactstrap";
import { AvForm, AvField, AvGroup, AvInput, AvFeedback, AvRadioGroup, AvRadio, AvCheckboxGroup, AvCheckbox } from 'availity-reactstrap-validation';
import Breadcrumbs from '../../components/Common/Breadcrumb';
import { withRouter, Link } from 'react-router-dom';
import { withTranslation } from 'react-i18next';
import stateWrapper from "../../containers/provider";
import instance from '../../helpers/axiosly';
import CONSTANTS from '../../App.constant';
import Page404 from '../Utility/pages-404';
import { configParams } from '../../config';
import "./football.scss";

const CompetitionDetails = (props) => {
    const [state, setState] = useState({
        errorPage: false,
        activeTab: '1',
    });
    const [competition, setCompetition] = useState({
        name: '',
        season: ''
    });

    const loadCompetition = async() => {
        try {
            let t = await props.footballStore.getCompetitionById(props.match?.params?.id);
            if (t == null) {
                setState({
                    errorPage: true
                })
                return;
            }

            console.log(t);

            setCompetition({...competition, ...t, season: t?.season.replace('/','-')});
            console.log(competition)
        } catch(e) { console.log(e) }
    }

    useEffect(async() => {
        loadCompetition();
    }, []);

    const toggleTab = (tab) =>{
        if (state.activeTab !== tab) {
            setState({
                activeTab: tab
            });
        }
    }


    return (
        <React.Fragment>
            <div className="page-content">
                <Container fluid>
                    {state.errorPage == false ? (
                        <>
                            <Row>
                                <Col lg={12} sm={12} xs={12}>
                                    <Card style={{backgroundColor: competition?.backgroundColor}}>
                                        <CardBody>
                                            <Nav pills justified>
                                                <NavItem>
                                                    <NavLink
                                                        className={classnames({ active: state.activeTab === '1' })}
                                                        onClick={() => { toggleTab('1'); }}
                                                    >
                                                        <i className="bx bx-chat font-size-20 d-sm-none"></i>
                                                        <span className="d-none d-sm-block">FIXTURES</span>
                                                    </NavLink>
                                                </NavItem>
                                                <NavItem>
                                                    <NavLink
                                                        className={classnames({ active: state.activeTab === '2' })}
                                                        onClick={() => { toggleTab('2'); }}
                                                    >
                                                        <i className="bx bx-group font-size-20 d-sm-none"></i>
                                                        <span className="d-none d-sm-block">STANDING</span>
                                                    </NavLink>
                                                </NavItem>
                                                <NavItem>
                                                    <NavLink
                                                        className={classnames({ active: state.activeTab === '3' })}
                                                        onClick={() => { toggleTab('3'); }}
                                                    >
                                                        <i className="bx bx-book-content font-size-20 d-sm-none"></i>
                                                        <span className="d-none d-sm-block">STATS</span>
                                                    </NavLink>
                                                </NavItem>
                                            </Nav>
                                        </CardBody>
                                    </Card>
                                </Col>
                            </Row>
                        </>
                        ) : 
                        (
                            <Page404 />
                        )
                    }
                </Container>
            </div>
        </React.Fragment>
    )
}

export default withRouter(withTranslation()(stateWrapper(CompetitionDetails)))
