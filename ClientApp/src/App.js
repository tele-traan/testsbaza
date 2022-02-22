import React, { Component } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import { AddTest } from './components/AddTest'
import { ManageTests } from './components/ManageTests'
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
        <Layout>
            <Route exact path='/' component={Home} />
            <Route exact path='/counter' component={Counter} />
            <Route exact path='/fetch-data' component={FetchData} />
            <Route exact path='/add-test' component={AddTest} />
            <Route exact path="/manage-tests" component={ManageTests} />
            <Route path="/edit-test" component={EditTest} />
            <Route exact path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
      </Layout>
    );
  }
}