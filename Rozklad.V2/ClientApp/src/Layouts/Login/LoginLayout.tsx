import React, {CSSProperties, useState} from 'react';
import Form from "./Form";
import TelegramLogin from "./TelegramLogin";
import dogLoginImage from "../../images/dog-login.jpeg";
import {url} from "inspector";
import GroupSelect from "./GroupSelect";
import {useLocation} from 'react-router-dom';

function useQuery() {
    return new URLSearchParams(useLocation().search);
}

function LoginLayout() {
    const query = useQuery()

    const renderAlert = () => {
        if (query.get("message")) {
            return (
                <div className="alert alert-warning alert-dismissible fade show" role="alert">
                    {query.get("message")}
                    <button type="button" className="close" data-dismiss="alert" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
            )
        }
    }
    return (
        <div className="container">
            <div className="row justify-content-center">
                <div className="col-md-9 col-lg-12 col-xl-10">
                    <div className="card shadow-lg o-hidden border-0 my-5">
                        <div className="card-body p-0">
                            <div className="row">
                                <div className="col-lg-6  d-lg-flex">
                                    <div className="flex-grow-1 bg-login-image"
                                         style={{
                                             backgroundImage: 'url(' + require('../../images/dog-login.jpeg') + ')',
                                             height: "400px"
                                         }}/>
                                </div>
                                <div className="col-lg-6">
                                    <div className="p-5">
                                        <div className="text-center">
                                            <h4 className="text-dark mb-4">Rozklad</h4>
                                        </div>
                                        {renderAlert()}
                                        <TelegramLogin/>

                                        <div className="text-center"/>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    );
}

export default LoginLayout;
