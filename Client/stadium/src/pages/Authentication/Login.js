import React, { useState, useEffect } from 'react';
import _ from 'lodash';
import { withRouter, Link } from 'react-router-dom';
import { Row, Col, CardBody, Card, Alert,Container } from "reactstrap";
import { AvForm, AvField } from 'availity-reactstrap-validation';
import "../../styles/Login.css";
import logo from "../../assets/Images/stadium logo neon.png";
import team1 from "../../assets/Images/team1.png";
import team2 from "../../assets/Images/team2.png";
import team3 from "../../assets/Images/team3.png";
import team4 from "../../assets/Images/team4.png";
import team5 from "../../assets/Images/team5.png";
import stadium from "../../assets/Images/stadium.png";

 const Login = (props) => {
    const [state, setState] = useState({
        loading: false
    });
    const stateSetting = (newState, key) => {
        const temp = {...state};
        temp[`${key}`] = newState;
        setState(temp);
    }
    // handleValidSubmit
  function  handleValidSubmit(event, values) {}
    return (
        <React.Fragment>            
            <div>
                <Link to="/">
                    <img className="logo" src={logo} alt="logo" />
                </Link>
            </div>
            <div className="account-pages my-5 pt-sm-5">
                <Container>
                    <div>
                        <img className="avatar  team1" src={team1} alt="team1" />
                        <img className="avatar team2" src={team2} alt="team2" />
                        <img className="avatar team3" src={team3} alt="team3" />
                        <img className="avatar team4" src={team4} alt="team4" />
                        <img className="avatar team5" src={team5} alt="team5" />
                    </div>
                    <Row className="justify-content-center overflow-hidden">
                        <Col className="box" style={{padding:0}} lg={9} md={12} sm={12}>
                            <Row>
                                <Col style={{padding: 30}} md={6} lg={4} xl={6} sm={12}>
                                        <Card className="overflow-hidden border-0">
                                            <div className="bg-soft-primary">
                                                <Row>
                                                    <Col className="col-12">
                                                        <div className="text-primary text-center text-dark">
                                                            <h5 className="text-primary text-dark">Welcome back !</h5>
                                                            <p>We're so excited to see you again!</p>
                                                        </div>
                                                    </Col>
                                                </Row>
                                            </div>
                                            <CardBody className="pt-0 border-0">
                                                <div className="p-2">

                                                    <AvForm className="form-horizontal" onValidSubmit={(e,v) => { handleValidSubmit(e,v) }}>

                                                        {props.error && props.error ? <Alert color="danger">{props.error}</Alert> : null}

                                                        <div className="form-group">
                                                            <AvField name="email" label="EMAIL" value="" className="form-control" placeholder="Enter email" type="email" required />
                                                        </div>

                                                        <div className="form-group">
                                                            <AvField name="password" label="PASSWORD" value="" type="password" required placeholder="Enter Password" />
                                                        </div>
                                                        <div className="mt-4">
                                                            <Link to="/forgot-password" className="text-muted"><i className="mdi mdi-lock mr-1"></i> Forgot your password?</Link>
                                                        </div>
                                                        <div className="mt-3">
                                                            <button disabled={state.loading} className="btn btn-primary btn-block waves-effect waves-light btn-dark" type="submit">
                                                                Login
                                                            </button>
                                                        </div>
                                                        <div className="mt-4">
                                                            Need an account?<Link to="/register" className="text-muted"><i className="mdi mdi-lock mr-1"></i> Register</Link>
                                                        </div>
                                                    </AvForm>
                                                </div>
                                            </CardBody>
                                        </Card>
                                    </Col> 
                                <Col md={6} lg={6} xl={6} sm={12}>
                                    <img className="stadium" src={stadium} alt="stadium" />
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                </Container>
            </div>
        </React.Fragment>
     );
    }

export default withRouter(Login)