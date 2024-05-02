import React, { useState } from 'react';
import {
  Box,
  Button,
  Container,
  Flex,
  FormControl,
  FormLabel,
  Heading,
  Input,
  Text,
  Grid,
  GridItem,
} from '@chakra-ui/react';

import dayjs from 'dayjs';

import useLogs from '../hooks/useLogs';

const Logs = () => {
  const { getLogs } = useLogs();
  const [logs, setLogs] = useState([]);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [params, setParams] = useState({
    from: '',
    to: '',
  });

  const handleSearch = async () => {
    try {
      const data = await getLogs(params);
      setLogs(data);
    } catch (error) {
      setError(error);
    }
  };

  return (
    <Box bg='gray.100' py={12} flexGrow={1}>
      <Container maxW='7xl'>
        <Heading color='teal.600' mb={4}>
          Логи
        </Heading>
        <Flex mb={4} gap={4}>
          <FormControl>
            <FormLabel htmlFor='from'>Початкова дата</FormLabel>
            <Input
              name='from'
              type='date'
              value={params.from}
              onChange={(e) => {
                setParams({
                  ...params,
                  from: e.target.value,
                });
              }}
            />
          </FormControl>
          <FormControl>
            <FormLabel htmlFor='to'>Кінцева дата</FormLabel>
            <Input
              name='to'
              type='date'
              value={params.to}
              onChange={(e) =>
                setParams({
                  ...params,
                  to: e.target.value,
                })
              }
            />
          </FormControl>
        </Flex>
        <Flex mb={4}>
          <Box>
            <Button
              colorScheme='teal'
              onClick={handleSearch}
              isLoading={isLoading}
            >
              Пошук
            </Button>
          </Box>
        </Flex>
        <Box
          maxH='3xl'
          backgroundColor='white'
          overflowY='auto'
          borderRadius='xl'
          p={4}
        >
          {logs && logs.length > 0 ? (
            <>
              <Grid templateColumns='repeat(4, 1fr)' gap={4} p={4}>
                <GridItem>
                  <Text fontWeight='bold' color='gray.600'>
                    Дата:
                  </Text>
                </GridItem>
                <GridItem>
                  <Text fontWeight='bold'>Дія</Text>
                </GridItem>
                <GridItem>
                  <Text>Опис</Text>
                </GridItem>
                <GridItem>
                  <Text>Користувач</Text>
                </GridItem>
              </Grid>
              {logs.map((log, index) => (
                <LogItem key={index} log={log} />
              ))}
            </>
          ) : (
            <Text>Немає даних</Text>
          )}
        </Box>
      </Container>
    </Box>
  );
};

const LogItem = ({ log }) => (
  <Grid templateColumns='repeat(4, 1fr)' gap={4} p={4}>
    <GridItem>
      <Text fontWeight='bold' color='gray.600'>
        [{dayjs(log.timestamp).format('YYYY-MM-DD HH:mm:ss')}]:
      </Text>
    </GridItem>
    <GridItem>
      <Text fontWeight='bold'>{log.actionType}</Text>
    </GridItem>
    <GridItem>
      <Text>{log.description}</Text>
    </GridItem>
    <GridItem>
      <Text fontStyle='italic' color='gray.500'>
        {log.userId}
      </Text>
    </GridItem>
  </Grid>
);

export default Logs;
